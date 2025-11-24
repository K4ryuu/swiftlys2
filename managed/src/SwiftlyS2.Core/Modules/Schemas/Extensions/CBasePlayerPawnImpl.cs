using SwiftlyS2.Core.Natives;
using SwiftlyS2.Core.Services;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Core.SchemaDefinitions;

internal partial class CBasePlayerPawnImpl : CBasePlayerPawn
{
    public void CommitSuicide( bool explode, bool force )
    {
        GameFunctions.PawnCommitSuicide(Address, explode, force);
    }

    public Vector? EyePosition {
        get {
            if (!IsValid) return null;
            if (AbsOrigin == null) return null;

            var absOrigin = AbsOrigin.Value;
            var viewmodelOffset = ViewOffset;
            if (viewmodelOffset == null) return null;

            absOrigin.Z += viewmodelOffset.Z.Value;
            return absOrigin;
        }
    }

    public float GroundDistance {
        get {
            if (!IsValid) return -1f;
            if (AbsOrigin == null) return -1f;

            var start = AbsOrigin.Value;
            var angle = new QAngle(90f, 0f, 0f);
            angle.ToDirectionVectors(out var fwd, out var _, out var _);
            var end = start + new Vector(
                fwd.X * 8192f,
                fwd.Y * 8192f,
                fwd.Z * 8192f
            );

            var trace = new CGameTrace();
            TraceManager.SimpleTrace(start, end, RayType_t.RAY_TYPE_HULL, RnQueryObjectSet.All, MaskTrace.Sky, MaskTrace.Empty, MaskTrace.Empty, CollisionGroup.Always, ref trace, Address, nint.Zero);

            return trace.Distance;
        }
    }

    public MaskTrace InteractsWith {
        get {
            return !IsValid ? MaskTrace.Empty : (MaskTrace)Collision.CollisionAttribute.InteractsWith;
        }
    }

    public MaskTrace InteractsAs {
        get {
            return !IsValid ? MaskTrace.Empty : (MaskTrace)Collision.CollisionAttribute.InteractsAs;
        }
    }

    public MaskTrace InteractsExclude {
        get {
            return !IsValid ? MaskTrace.Empty : (MaskTrace)Collision.CollisionAttribute.InteractsExclude;
        }
    }
}