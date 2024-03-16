using Constant;
using MainGame.ModLoader.Glb;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace MainGame.UnityView.Control.MouseKeyboard
{
    public class BlockClickDetect : MonoBehaviour, IBlockClickDetect
    {
        private Camera _mainCamera;

        public bool TryGetCursorOnBlockPosition(out Vector3Int position)
        {
            position = Vector3Int.zero;

            if (!TryGetCursorOnBlock(out var blockObject)) return false;


            position = blockObject.BlockPosition;

            return true;
        }

        public bool TryGetClickBlock(out BlockGameObject blockObject)
        {
            blockObject = null;
            // UIのクリックかどうかを判定
            if (EventSystem.current.IsPointerOverGameObject()) return false;
            if (InputManager.Playable.ScreenLeftClick.GetKeyDown && TryGetCursorOnBlock(out blockObject)) return true;

            blockObject = null;
            return false;
        }

        public bool TryGetClickBlockPosition(out Vector3Int position)
        {
            if (InputManager.Playable.ScreenLeftClick.GetKeyDown && TryGetCursorOnBlockPosition(out position)) return true;

            position = Vector3Int.zero;
            return false;
        }

        [Inject]
        public void Construct(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }


        private bool TryGetCursorOnBlock(out BlockGameObject blockObject)
        {
            blockObject = null;

            var ray = _mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));

            if (!Physics.Raycast(ray, out var hit, 100, LayerConst.BlockOnlyLayerMask)) return false;
            var child = hit.collider.gameObject.GetComponent<BlockGameObjectChild>();
            if (child is null) return false;


            blockObject = child.BlockGameObject;

            return true;
        }
    }
}