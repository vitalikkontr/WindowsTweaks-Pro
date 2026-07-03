using System;

namespace WindowsTweaks
{
    internal sealed class TweakAction
    {
        public string Description { get; }
        public Action Apply { get; }
        public Action Revert { get; }

        public TweakAction(string description, Action apply, Action revert)
        {
            Description = description;
            Apply = apply;
            Revert = revert;
        }
    }
}
