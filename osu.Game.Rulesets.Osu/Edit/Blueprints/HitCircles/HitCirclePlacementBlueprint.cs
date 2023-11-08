// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.Events;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Osu.Edit.Blueprints.HitCircles.Components;
using osu.Game.Rulesets.Osu.Objects;
using osuTK.Input;

namespace osu.Game.Rulesets.Osu.Edit.Blueprints.HitCircles
{
    public partial class HitCirclePlacementBlueprint : PlacementBlueprint
    {
        public new HitCircle HitObject => (HitCircle)base.HitObject;

        private readonly HitCirclePiece circlePiece;

        public HitCirclePlacementBlueprint()
            : base(new HitCircle())
        {
            InternalChild = circlePiece = new HitCirclePiece();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            BeginPlacement();
        }

        protected override void Update()
        {
            base.Update();

            circlePiece.UpdateFrom(HitObject);
        }
        protected override bool OnDragStart(DragStartEvent e)
        {
            // for drag seeking
            if (e.Button == MouseButton.Middle)
            {
                return false;
            }
            if (e.Button == MouseButton.Left && e.IsPressed(MouseButton.Middle))
            {
                return false;
            }
            return base.OnDragStart(e);
        }
        protected override bool OnMouseDown(MouseDownEvent e)
        {
            // return to select mode with right btn
            if (e.Button == MouseButton.Right && !e.ShiftPressed)
            {
                AutoMapper.Internal.TypeExtensions.GetInheritedMethod(typeof(OsuHitObjectComposer), "setSelectTool")?.Invoke(
                    Framework.Graphics.DrawableExtensions.FindClosestParent<OsuHitObjectComposer>(this),
                    null
                );
                return true;
            }
            // for drag seeking
            if (e.Button == MouseButton.Middle)
            {
                return false;
            }
            if (e.Button == MouseButton.Left && e.IsPressed(MouseButton.Middle))
            {
                return false;
            }
            if (e.Button == MouseButton.Left)
            {
                EndPlacement(true);
                return true;
            }

            return base.OnMouseDown(e);
        }

        public override void UpdateTimeAndPosition(SnapResult result)
        {
            base.UpdateTimeAndPosition(result);
            HitObject.Position = ToLocalSpace(result.ScreenSpacePosition);
        }
    }
}
