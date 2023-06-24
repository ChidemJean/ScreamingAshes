using Godot;
using System;

namespace ChidemGames.Extensions
{
   public static class TransformExtensions
   {
      public static Transform LookAtBasis(this Transform owner, Vector3 target)
      {
         Basis lookingAtBasis = new Basis();
         lookingAtBasis.z = target - owner.origin;
         lookingAtBasis.x = Vector3.Up.Cross(lookingAtBasis.z);
         lookingAtBasis.y = lookingAtBasis.z.Cross(lookingAtBasis.x);
         lookingAtBasis = lookingAtBasis.Orthonormalized();

         owner.basis = lookingAtBasis;

         return owner;
      }

		public static Transform AlignBasis(this Transform owner, Vector3 target)
      {
         Basis lookingAtBasis = new Basis();
         lookingAtBasis.z = target - owner.origin;
         lookingAtBasis.x = Vector3.Up.Cross(lookingAtBasis.z);
         lookingAtBasis.y = lookingAtBasis.z.Cross(lookingAtBasis.x);
         lookingAtBasis = lookingAtBasis.Orthonormalized();

         owner.basis = lookingAtBasis;

         return owner;
      }

		public static Transform ChangeBasis(this Transform owner, Basis basisTarget)
		{
			owner.basis = basisTarget;
			return owner;
		}

		public static Transform LookAtWithY(this Transform owner, Vector3 newY, Vector3 vUp)
		{
			owner.basis.y = newY.Normalized();
			owner.basis.z = vUp * -1;
			owner.basis.x = owner.basis.z.Cross(owner.basis.y).Normalized();
			owner.basis.z = owner.basis.y.Cross(owner.basis.x).Normalized();
			owner.basis.x = owner.basis.x * -1;
			owner.basis = owner.basis.Orthonormalized();

			return owner;
		}
   }
}