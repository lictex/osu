// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Edit.Blueprints.HitCircles;
using osu.Game.Rulesets.Osu.Edit.Blueprints.Sliders;
using osu.Game.Rulesets.Osu.Edit.Blueprints.Spinners;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class OsuBlueprintContainer : ComposeBlueprintContainer
    {
        public OsuBlueprintContainer(HitObjectComposer composer)
            : base(composer)
        {
        }

        protected override SelectionHandler<HitObject> CreateSelectionHandler() => new OsuSelectionHandler();

        public override HitObjectSelectionBlueprint? CreateHitObjectBlueprintFor(HitObject hitObject)
        {
            switch (hitObject)
            {
                case HitCircle circle:
                    return new HitCircleSelectionBlueprint(circle);

                case Slider slider:
                    return new SliderSelectionBlueprint(slider);

                case Spinner spinner:
                    return new SpinnerSelectionBlueprint(spinner);
            }

            return base.CreateHitObjectBlueprintFor(hitObject);
        }

        // drag mmb+lmb to seek around
        // actually made for pen buttons
        private bool seekDrag = false;
        private float buffer = .0f;

        protected override bool OnDragStart(Framework.Input.Events.DragStartEvent e)
        {
            if (e.Button == osuTK.Input.MouseButton.Left && e.IsPressed(osuTK.Input.MouseButton.Middle))
            {
                return seekDrag = true;
            }

            return base.OnDragStart(e);
        }

        protected override void OnDrag(Framework.Input.Events.DragEvent e)
        {
            if (seekDrag)
            {
                buffer += -e.Delta.X;
                buffer += -e.Delta.Y;
                if (System.Math.Abs(buffer) > 5.0f)
                {
                    typeof(Screens.Edit.Editor).GetMethod(
                        "seek",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                    )?.Invoke(
                        Framework.Graphics.DrawableExtensions.FindClosestParent<Screens.Edit.Editor>(this),
                        new object[] { e, System.Math.Sign(buffer) }
                    );
                    buffer = .0f;
                }
            }
            else
            {
                base.OnDrag(e);
            }
        }

        protected override void OnDragEnd(Framework.Input.Events.DragEndEvent e)
        {
            seekDrag = false;
            buffer = .0f;
            base.OnDragEnd(e);
        }

        protected override bool OnMouseDown(Framework.Input.Events.MouseDownEvent e)
        {
            if (e.Button == osuTK.Input.MouseButton.Middle)
            {
                return false;
            }
            if (e.Button == osuTK.Input.MouseButton.Left && e.IsPressed(osuTK.Input.MouseButton.Middle))
            {
                return false;
            }
            return base.OnMouseDown(e);
        }
    }
}
