using UnityEngine;
using VRC.SDK3.ClientSim;
using VRC.Udon.Common;

namespace Hai.Myrddin.ClientSimVR.Runtime
{
    /// Myrddin Tracking Provider.<br/>
    /// A ClientSim tracking provider that integrates with Myrddin.<br/>
    /// Use the prefab in /Internal/ClientSimMyrddinTrackingData.prefab
    public class ClientSimMyrddinTrackingProvider : ClientSimTrackingProviderBase
    {
        public override Transform GetHandRaycastTransform(HandType handType) => handType == HandType.RIGHT ? rightHand : leftHand;

        public override bool IsVR() => true;

        public override bool SupportsPickupAutoHold() => false;

        public override void LookTowardsPoint(Vector3 point) { }
    }
}