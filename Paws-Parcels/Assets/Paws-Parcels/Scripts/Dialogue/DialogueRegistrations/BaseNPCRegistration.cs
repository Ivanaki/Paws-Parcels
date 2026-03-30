using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Zenject;

namespace NPCDialogs
{
    public abstract class BaseNPCRegistration : MonoBehaviour
    {
        [Inject] private DialogueRegistrations _dialogueRegistrations;

        private readonly List<(string, Action<DialogueRunner>)> _commands = new();
        private readonly List<(string, Action<DialogueRunner>)> _functions = new();
        
        protected abstract void Bind();
        
        protected void RegisterCommand(string commandName, Action<DialogueRunner> registerAction)
        {
            _commands.Add((commandName, registerAction));
            if(gameObject.activeInHierarchy)
                _dialogueRegistrations.RegisterCommand(commandName, registerAction);
        }

        protected void RegisterFunction(string functionName, Action<DialogueRunner> registerAction)
        {
            _functions.Add((functionName, registerAction));
            if(gameObject.activeInHierarchy)
                _dialogueRegistrations.RegisterFunction(functionName, registerAction);   
        }

        protected virtual void OnEnable()
        {
            _commands.ForEach(x => _dialogueRegistrations.RegisterCommand(x.Item1, x.Item2));
            _functions.ForEach(x => _dialogueRegistrations.RegisterFunction(x.Item1, x.Item2));
        }
        
        protected virtual void Start()
        {
            Bind();
        }
        
        protected virtual void OnDisable()
        {
            _commands.ForEach(x => _dialogueRegistrations.UnregisterCommand(x.Item1));
            _functions.ForEach(x => _dialogueRegistrations.UnregisterFunction(x.Item1));
        }
    }
}