using Godot;
using System;

namespace ChidemGames.Extensions
{
   public static class TransformExtensions
   {
      public static Transform3D LookAtBasis(this Transform3D owner, Vector3 target)
      {
         Basis lookingAtBasis = new Basis();
         lookingAtBasis.Z = target - owner.Origin;
         lookingAtBasis.X = Vector3.Up.Cross(lookingAtBasis.Z);
         lookingAtBasis.Y = lookingAtBasis.Z.Cross(lookingAtBasis.X);
         lookingAtBasis = lookingAtBasis.Orthonormalized();

         owner.Basis = lookingAtBasis;

         return owner;
      }

		public static Transform3D AlignBasis(this Transform3D owner, Vector3 target)
      {
         Basis lookingAtBasis = new Basis();
         lookingAtBasis.Z = target - owner.Origin;
         lookingAtBasis.X = Vector3.Up.Cross(lookingAtBasis.Z);
         lookingAtBasis.Y = lookingAtBasis.Z.Cross(lookingAtBasis.X);
         lookingAtBasis = lookingAtBasis.Orthonormalized();

         owner.Basis = lookingAtBasis;

         return owner;
      }

		public static Transform3D ChangeBasis(this Transform3D owner, Basis basisTarget)
		{
			owner.Basis = basisTarget;
			return owner;
		}

		public static Transform3D LookAtWithY(this Transform3D owner, Vector3 newY, Vector3 vUp)
		{
			owner.Basis.Y = newY.Normalized();
			owner.Basis.Z = vUp * -1;
			owner.Basis.X = owner.Basis.Z.Cross(owner.Basis.Y).Normalized();
			owner.Basis.Z = owner.Basis.Y.Cross(owner.Basis.X).Normalized();
			owner.Basis.X = owner.Basis.X * -1;
			owner.Basis = owner.Basis.Orthonormalized();

			return owner;
		}
   }
}