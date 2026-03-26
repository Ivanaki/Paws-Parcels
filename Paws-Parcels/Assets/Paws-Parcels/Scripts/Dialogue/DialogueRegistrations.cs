using System;
using System.Collections.Generic;
using ObservableCollections;
using UnityEngine;
using Yarn.Unity;

namespace NPCDialogs
{
    public class DialogueRegistrations
    {
        public IReadOnlyObservableDictionary<string, Action<DialogueRunner>> CommandsRegistrations => _commandsRegistrations;
        public IReadOnlyObservableDictionary<string, Action<DialogueRunner>> FunctionsRegistrations => _functionsRegistrations;
        
        private readonly ObservableDictionary<string, Action<DialogueRunner>> _commandsRegistrations = new();
        private readonly ObservableDictionary<string, Action<DialogueRunner>> _functionsRegistrations = new();

        public void AddCommand(string commandName, Action<DialogueRunner> registerAction)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                Debug.LogError("Command name is null or empty");
                return;
            }

            if (registerAction == null)
            {
                Debug.LogError($"Register action for command '{commandName}' is null");
                return;
            }

            if (_commandsRegistrations.ContainsKey(commandName))
            {
                Debug.LogError($"Command name '{commandName}' is already registered");
                return;
            }

            _commandsRegistrations[commandName] = registerAction;
        }

        public void RemoveCommand(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                Debug.LogError("Command name is null or empty");
                return;
            }

            if (_commandsRegistrations.ContainsKey(commandName) == false)
            {
                Debug.LogError($"Command name '{commandName}' is not registered");
                return;
            }

            _commandsRegistrations.Remove(commandName);
        }

        public void AddFunction(string functionName, Action<DialogueRunner> registerAction)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                Debug.LogError("Function name is null or empty");
                return;
            }

            if (registerAction == null)
            {
                Debug.LogError($"Register action for function '{functionName}' is null");
                return;
            }

            if (_functionsRegistrations.ContainsKey(functionName))
            {
                Debug.LogError($"Function name '{functionName}' is already registered");
                return;
            }

            _functionsRegistrations[functionName] = registerAction;
        }

        public void RemoveFunction(string functionName)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                Debug.LogError("Function name is null or empty");
                return;
            }

            if (_functionsRegistrations.ContainsKey(functionName) == false)
            {
                Debug.LogError($"Function name '{functionName}' is not registered");
                return;
            }

            _functionsRegistrations.Remove(functionName);
        }
    }
}