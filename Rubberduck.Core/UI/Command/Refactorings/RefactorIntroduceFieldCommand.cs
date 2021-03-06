﻿using Rubberduck.Common;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.Refactorings.IntroduceField;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck.UI.Command.Refactorings
{
    public class RefactorIntroduceFieldCommand : RefactorCommandBase
    {
        private readonly RubberduckParserState _state;
        private readonly IMessageBox _messageBox;

        public RefactorIntroduceFieldCommand (IVBE vbe, RubberduckParserState state, IMessageBox messageBox)
            :base(vbe)
        {
            _state = state;
            _messageBox = messageBox;
        }

        protected override bool EvaluateCanExecute(object parameter)
        {
            if (_state.Status != ParserState.Ready)
            {
                return false;
            }

            var selection = Vbe.GetActiveSelection();

            if (!selection.HasValue)
            {
                return false;
            }

            var target = _state.AllUserDeclarations.FindVariable(selection.Value);

            return target != null 
                && !_state.IsNewOrModified(target.QualifiedModuleName)
                && target.ParentScopeDeclaration.DeclarationType.HasFlag(DeclarationType.Member);
        }

        protected override void OnExecute(object parameter)
        {
            var selection = Vbe.GetActiveSelection();

            if (!selection.HasValue)
            {
                return;
            }

            var refactoring = new IntroduceFieldRefactoring(Vbe, _state, _messageBox);
            refactoring.Refactor(selection.Value);
        }
    }
}
