﻿/*
 * Copyright 2019 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behaviors;

namespace BehaviorProperties
{
  public enum PropType
  {
    Number,
    Decimal,
    Boolean,
    Actor,
    String,
    CardDeck,
    Prefab,
    Sound,
    ParticleEffect,
    ActorGroup,
    Image,
    Color,
    Enum,
    NumberArray,
    StringArray,
    EnumArray,
    ActorArray
  }

  // This is exposed in APIv2 (see PropDeckOptions in properties.js.txt).
  // DO NOT MODIFY names or types, as that might break existing games.
  // If you have to, you can modify things here and do the appropriate backward-compatibility
  // conversions in the prop...() functions in properties.js.txt.
  [System.Serializable]
  public struct DeckOptions
  {
    public string cardCategory;
    public string iconResPath;
    public string[] defaultCardURIs;
    public bool oneCardOnly;
  }

  [System.Serializable]
  public struct EnumAllowedValue
  {
    public string value;
    public string label;
  }

  // This should match with whatever the propNumber, etc APIv2 functions produce.
  // This is a superset of the PropOptions type in APIv2 (see properties.js.txt).
  // DO NOT MODIFY names or types, as that might break existing games.
  // If you have to, you can modify things here and do the appropriate backward-compatibility
  // conversions in the prop...() functions in properties.js.txt.
  [System.Serializable]
  public struct PropDef
  {
    // FIELDS MUST MATCH JAVASCRIPT! (see properties.js.txt).

    // Required (generated by the prop...() functions):
    public string type;
    public string variableName;
    public string defaultValueString;

    // Extra options that can be specified by the user:
    public string label; // For the UI. Can use spaces, etc.
    public string comment; // TODO: how is this different from label?
    public DeckOptions deckOptions;
    public string pickerPrompt;
    public bool allowOffstageActors;
    public PropDefRequirement[] requires;
    public EnumAllowedValue[] allowedValues;

    public PropType GetPropType() { return Util.ParseEnum<PropType>(type); }
  }

  [System.Serializable]
  public struct PropDefRequirement
  {
    public string key;
    public string value;
    public string op;
  }

  public static class PropUtil
  {

    public static object GetPropertyValue(PropertyAssignment assignment, PropType type)
    {
      switch (type)
      {
        case PropType.Number:
          return assignment.GetValue<int>();
        case PropType.Decimal:
          return assignment.GetValue<float>();
        case PropType.Boolean:
          return assignment.GetValue<bool>();
        case PropType.Actor:
          return assignment.GetValue<string>();
        case PropType.Sound:
          return assignment.GetValue<string>();
        case PropType.ParticleEffect:
          return assignment.GetValue<string>();
        case PropType.CardDeck:
          return assignment.GetValue<string[]>() ?? new string[0];
        case PropType.String:
          return assignment.GetValue<string>();
        case PropType.Prefab:
          return assignment.GetValue<string>();
        case PropType.ActorGroup:
          return assignment.GetValue<string>();
        case PropType.Image:
          return assignment.GetValue<string>();
        case PropType.Color:
          return assignment.GetValue<string>();
        case PropType.Enum:
          return assignment.GetValue<string>();
        case PropType.NumberArray:
          return assignment.GetValue<int[]>() ?? new int[0];
        case PropType.ActorArray:
        case PropType.StringArray:
        case PropType.EnumArray:
          return assignment.GetValue<string[]>() ?? new string[0];
        default:
          throw new System.Exception($"Didn't expect to get property of type {type}. Property var name: {assignment.propertyName}");
      }
    }

    // TODO rewrite as just.. ToAssignment(PropEditor editable)
    public static PropertyAssignment ToAssignment(PropEditor editable)
    {
      var assign = new PropertyAssignment();
      assign.propertyName = editable.propDef.variableName;

      switch (editable.propType)
      {
        case PropType.Number:
          assign.SetValue<int>((int)editable.data);
          break;
        case PropType.Decimal:
          assign.SetValue<float>((float)editable.data);
          break;
        case PropType.Boolean:
          assign.SetValue<bool>((bool)editable.data);
          break;
        case PropType.String:
          assign.SetValue<string>((string)editable.data);
          break;
        case PropType.CardDeck:
          assign.SetValue<string[]>((string[])editable.data);
          break;
        case PropType.Actor:
          if (editable.data == null)
          {
            assign.SetValue<string>(null);
          }
          else
          {
            assign.SetValue<string>((string)editable.data);
          }
          break;
        case PropType.Sound:
          assign.SetValue<string>((string)editable.data);
          break;
        case PropType.ParticleEffect:
          assign.SetValue<string>((string)editable.data);
          break;
        case PropType.Prefab:
          assign.SetValue<string>((string)editable.data);
          break;
        case PropType.ActorGroup:
          assign.SetValue<string>((string)editable.data);
          break;
        case PropType.Image:
          assign.SetValue<string>((string)editable.data);
          break;
        case PropType.Color:
          assign.SetValue<string>((string)editable.data);
          break;
        case PropType.Enum:
          assign.SetValue<string>((string)editable.data);
          break;
        case PropType.NumberArray:
          assign.SetValue<int[]>((int[])editable.data);
          break;
        case PropType.StringArray:
        case PropType.EnumArray:
        case PropType.ActorArray:
          assign.SetValue<string[]>((string[])editable.data);
          break;
        default:
          throw new System.NotImplementedException();
      }

      return assign;
    }

    public static PropertyAssignment[] SerializeProps(PropEditor[] editableProps)
    {
      List<PropertyAssignment> rv = new List<PropertyAssignment>();

      foreach (PropEditor field in editableProps)
      {
        var assign = ToAssignment(field);
        rv.Add(assign);
      }

      return rv.ToArray();
    }

    public static System.Type GetExpectedType(PropType type)
    {
      switch (type)
      {
        case PropType.Number:
          return typeof(int);
        case PropType.Decimal:
          return typeof(float);
        case PropType.String:
          return typeof(string);
        case PropType.Boolean:
          return typeof(bool);
        case PropType.Actor:
          return typeof(string);
        case PropType.CardDeck:
          return typeof(string[]);
        case PropType.Prefab:
          return typeof(string);
        case PropType.Sound:
          return typeof(string);
        case PropType.ParticleEffect:
          return typeof(string);
        case PropType.ActorGroup:
          return typeof(string);
        case PropType.Enum:
          return typeof(string);
        case PropType.Image:
          return typeof(string);
        case PropType.Color:
          return typeof(string);
        case PropType.NumberArray:
          return typeof(int[]);
        case PropType.StringArray:
        case PropType.EnumArray:
        case PropType.ActorArray:
          return typeof(string[]);
        default:
          throw new System.NotImplementedException();
      }
    }

    public static object GetDefaultValue(PropType type)
    {
      switch (type)
      {
        case PropType.Number:
          return 0;
        case PropType.Decimal:
          return 1f;
        case PropType.String:
          return "";
        case PropType.Boolean:
          return false;
        case PropType.Actor:
          return null;
        case PropType.Prefab:
          return "";
        case PropType.CardDeck:
          return new string[0];
        case PropType.Sound:
          return "";
        case PropType.ParticleEffect:
          return "";
        case PropType.ActorGroup:
          return "";
        case PropType.Image:
          return "";
        case PropType.Color:
          return "#ffffff";
        case PropType.Enum:
          return "";
        case PropType.NumberArray:
          return new int[0];
        case PropType.StringArray:
        case PropType.EnumArray:
        case PropType.ActorArray:
          return new string[0];
        default:
          throw new System.NotImplementedException();
      }
    }

    public static object ParsePropertyInitialValueOrDefault(PropType type, string valueString)
    {
      if (valueString.IsNullOrEmpty())
      {
        return GetDefaultValue(type);
      }
      else
      {
        try
        {
          switch (type)
          {
            case PropType.Number:
              return System.Int32.Parse(valueString);
            case PropType.Decimal:
              return System.Single.Parse(valueString);
            case PropType.String:
              return valueString;
            case PropType.Boolean:
              return valueString == "true";
            case PropType.Actor:
              return valueString;
            case PropType.Sound:
              return valueString;
            case PropType.ParticleEffect:
              return valueString;
            case PropType.ActorGroup:
              return valueString;
            case PropType.Enum:
              return valueString;
            case PropType.Image:
              return valueString;
            case PropType.Color:
              return valueString;
            case PropType.NumberArray:
              string numberValueString = $"{{\"value\": {valueString}}}";
              return JsonUtility.FromJson<NumberArrayObject>(numberValueString).value;
            case PropType.StringArray:
            case PropType.EnumArray:
            case PropType.ActorArray:
              string stringValueString = $"{{\"value\": {valueString}}}";
              return JsonUtility.FromJson<StringArrayObject>(stringValueString).value;
            case PropType.CardDeck:
            default:
              throw new System.Exception($"We should never be parsing an initial value for a prop of type {type} from property declarations.");
          }
        }
        catch (System.FormatException)
        {
          Util.LogError($"Failed to parse value string: '{valueString}'. Will return default value for prop type {type}.");
          return GetDefaultValue(type);
        }
      }

    }

    [Serializable]
    private class StringArrayObject
    {
      public string[] value;
    }

    [Serializable]
    private class NumberArrayObject
    {
      public int[] value;
    }
  }
}