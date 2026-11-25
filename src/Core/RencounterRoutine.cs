using System.Collections;
using UnityEngine;

namespace DeviceOfHermes;

/// <summary>Routine set on Rencounter</summary>
public static class RencounterRoutine
{
    extension(BattleUnitView self)
    {
        IEnumerator MoveTo(Vector3 dst, float margin = 0.1f)
        {
            var facing = (dst - self.WorldPosition).normalized;
            var velo = Vector3.zero;

            dst.y = Mathf.Min(dst.y, 0f);

            while (Vector3.Distance(self.WorldPosition, dst) >= margin)
            {
                self.WorldPosition = Vector3.SmoothDamp(self.WorldPosition, dst, ref velo, 0.035f);

                if (self.IsWallFront(facing))
                {
                    yield break;
                }

                yield return null;
            }

            yield break;
        }

        IEnumerator StartTo(Vector3 facing, float len, float rotate = 0f)
        {
            var dst = self.WorldPosition + Quaternion.AngleAxis(rotate, Vector3.up) * facing * len;

            return self.MoveTo(dst);
        }

        /// <summary>Move forward to target</summary>
        public IEnumerator MoveForward(BattleUnitView target, float forward, float rotate = 0f) => self.StartTo((target.WorldPosition - self.WorldPosition).normalized, forward, rotate);

        /// <summary>Move back to target</summary>
        public IEnumerator MoveBack(BattleUnitView target, float back, float rotate = 0f) => self.StartTo((self.WorldPosition - target.WorldPosition).normalized, back, rotate);

        /// <summary>Move to opponent</summary>
        public IEnumerator MoveOpponent(BattleUnitView target, float margin = 0.1f) => self.MoveTo(target.WorldPosition, margin);

        IEnumerator Teleport(Vector3 dst, Vector3 facing, float shift, float rotate)
        {
            dst -= Quaternion.AngleAxis(rotate, Vector3.up) * facing * HexagonalMapManager.Instance.tileSize * shift;

            dst.y = Mathf.Min(dst.y, 0f);

            if (self.IsWall(dst))
            {
                yield break;
            }

            self.WorldPosition = dst;

            yield break;
        }

        /// <summary>Teleport to opponent front</summary>
        public IEnumerator TeleportFront(BattleUnitView target, float distance, float rotate = 0f) => self.Teleport(target.WorldPosition, (target.WorldPosition - self.WorldPosition).normalized, distance, rotate);

        /// <summary>Teleport to opponent back</summary>
        public IEnumerator TeleportBack(BattleUnitView target, float distance, float rotate = 0f) => self.Teleport(target.WorldPosition, (self.WorldPosition - target.WorldPosition).normalized, distance, rotate);

        /// <summary>Moves by knocks</summary>
        public IEnumerator Knockback(BattleUnitView given, float power = 5f, float knock = 5f)
        {
            self.model.moveDetail.Knockback(given.model, power, 50f, knock, false);

            while (!self.model.moveDetail.isArrived)
            {
                yield return null;
            }

            yield break;
        }

        bool IsWallFront(Vector3 facing)
        {
            return self.IsWall(self.WorldPosition + facing * HexagonalMapManager.Instance.tileSize * 2);
        }

        bool IsWall(Vector3 at)
        {
            return HexagonalMapManager.Instance.IsWall(at) != HexagonalMapManager.WallDirection.NONE;
        }
    }
}
