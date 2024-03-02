﻿using System;
using System.Collections.Generic;
using Core.Item.Config;
using Cysharp.Threading.Tasks;
using MainGame.ModLoader;
using MainGame.ModLoader.Texture;
using ServerServiceProvider;
using UnityEngine;

namespace MainGame.UnityView.Item
{
    /// <summary>
    /// TODO ここもItemConfigと統合して、クライアント向けのリソースも含めたアイテムリストを作りたい
    /// TODO staticにするべき？
    /// </summary>
    public class ItemImageContainer
    {
        private readonly List<ItemViewData> _itemImageList = new();
        private readonly ItemViewData _nothingIndexItemImage;

        public ItemImageContainer(ModDirectory modDirectory, MoorestechServerServiceProvider moorestechServerServiceProvider)
        {
            _nothingIndexItemImage = new ItemViewData(null, null, new ItemConfigData("Not item", 100, "Not mod", 0));

            LoadTexture(modDirectory, moorestechServerServiceProvider).Forget();
        }

        //TODO これを消す
        [Obsolete("将来的には依存関係がすべて解決された時点で画像がロードされているようにしたいな、、")]
        public event Action OnLoadFinished;

        /// <summary>
        ///  テクスチャのロードは別スレッドで非同期で行いたいのでUniTaskをつける
        /// </summary>
        private async UniTask LoadTexture(ModDirectory modDirectory, MoorestechServerServiceProvider moorestechServerServiceProvider)
        {
            //await BlockGlbLoader.GetBlockLoaderは同期処理になっているため、ここで1フレーム待って他のイベントが追加されるのを待つ
            await UniTask.WaitForFixedUpdate();

            _itemImageList.Add(_nothingIndexItemImage); //id 0番は何もないことを表すのでnullを入れる
            
            var textures = ItemTextureLoader.GetItemTexture(modDirectory.Directory, moorestechServerServiceProvider);
            _itemImageList.AddRange(textures);

            OnLoadFinished?.Invoke();
        }


        public ItemViewData GetItemView(int itemId)
        {
            if (_itemImageList.Count <= itemId)
            {
                Debug.Log("存在しないアイテムIDです。" + itemId);
                return _nothingIndexItemImage;
            }
            

            return _itemImageList[itemId];
        }

    }
}