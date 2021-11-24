﻿using System.Collections.Generic;

namespace Core.Item.Util
{
    public static class CreateEmptyItemStacksList
    {
        public static List<IItemStack> Create(int amount)
        {
            var a = new List<IItemStack>();
            for (var i = 0; i < amount; i++)
            {
                a.Add(ItemStackFactory.CreatEmpty());
            }
            return a;
        }
    }
}