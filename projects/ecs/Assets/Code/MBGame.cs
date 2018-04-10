
/********************************************************************
created:    2018-04-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using Unique;
using UnityEngine;

namespace Client
{
    internal class BattleEntity : Entity
    {

    }

    internal class MovePart : Part, ITickable
    {
        void ITickable.Tick()
        {
            var transformPart = GetEntity().GetPart(typeof(TransformPart)) as TransformPart;
            if (null != transformPart)
            {
                var oldPosition = transformPart.GetPosition();
                var newPosition = oldPosition + direction * speed * Time.deltaTime;
                transformPart.SetPosition(newPosition);
            }
        }

        public Vector3 direction = Vector3.right;
        public float speed = 1.0f;
    }

    internal class TransformPart : Part
    {
        protected override void _DoInitPart()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _transform = go.transform;
        }

        protected override void _DoDispose()
        {
            GameObject.Destroy(_transform.gameObject);
        }

        public void SetPosition(Vector3 position)
        {
            if (null != _transform)
            {
                _transform.position = position;
            }
        }

        public Vector3 GetPosition()
        {
            if (null != _transform)
            {
                return _transform.position;
            }

            return Vector3.zero;
        }

        private Transform _transform;
    }

    public class MBGame : MonoBehaviour
    {
        IEnumerator Start()
        {
            _battle.AddPart(typeof(TransformPart));
            Vector3 direction = Vector3.right;

            while (true)
            {
                var movePart = _battle.AddPart(typeof(MovePart)) as MovePart;
                movePart.direction = direction;
                direction = -direction;

                yield return new WaitForSeconds(1.0f);
                _battle.RemovePart(typeof(MovePart));

                yield return new WaitForSeconds(1.0f);
            }

        }

        private void Update()
        {
            PartTickSystem.Instance.Tick();
        }

        private readonly BattleEntity _battle = new BattleEntity();
    }
}