﻿using System.Collections.Generic;
using System.Linq;
using Core.Config.Recipe;
using Core.Item;
using NUnit.Framework;

namespace Test.UnitTest.Core.Block
{
    public class MachineRecipeConfigTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(0,1000)]
        [TestCase(1,1500)]
        public void RecipeTimeTest(int id,int ans)
        {
            var time = MachineRecipeConfig.GetRecipeData(id).Time;
            Assert.AreEqual(ans,time);
        }

        
        [TestCase(0,0)]
        [TestCase(1,2)]
        public void RecipeInputItemIdTest(int id, int ans)
        {
            Assert.AreEqual(ans,MachineRecipeConfig.GetRecipeData(id).ItemInputs[0].Id);
        }
        
        /// <summary>
        /// レシピがある時のテスト
        /// </summary>
        [TestCase(0,new int[1]{0},1,1)]
        [TestCase(0,new int[1]{2},1,1)]
        [TestCase(1,new int[2]{1,2},3,1)]
        [TestCase(1,new int[2]{2,1},3,1)]
        [TestCase(3,new int[3]{1,2,0},5,1)]
        [TestCase(3,new int[3]{2,1,0},5,1)]
        public void RecipeInputItemBlockIdTest(int BlocksId, int[] items,int output0Id,double output0Percent)
        {
            var input = new List<IItemStack>();
            items.ToList().ForEach(
                i => input.Add(new ItemStack(i,1)));
            
            var ans = MachineRecipeConfig.GetRecipeData(BlocksId, input);
            Assert.AreEqual(output0Id,ans.ItemOutputs[0].OutputItem.Id);
            Assert.AreEqual(output0Percent,ans.ItemOutputs[0].Percent);
        }
        
        //nullの時のテスト
        [TestCase(3,new int[4]{2,1,0,5},0)]
        [TestCase(0,new int[3]{2,1,0},0)]
        [TestCase(3,new int[3]{4,1,0},0)]
        [TestCase(3,new int[2]{2,1},0)]
        [TestCase(10,new int[1]{2},0)]
        [TestCase(0,new int[0],0)]
        [TestCase(3,new int[3]{2,1,0},2)]//存在するときのテストケース
        [TestCase(1,new int[2]{2,1},1)]
        [TestCase(0,new int[1]{0},1)]
        public void NullRecipeTest(int BlocksId, int[] items,int outputLength)
        {
            var input = new List<IItemStack>();
            items.ToList().ForEach(
                i => input.Add(new ItemStack(i,1)));
            
            int ans = MachineRecipeConfig.GetRecipeData(BlocksId, input).ItemOutputs.Count;
            Assert.AreEqual(outputLength,ans);
        }

        [TestCase(0, new int[1] {0}, new int[1] {1}, true)]
        [TestCase(0, new int[1] {10}, new int[1] {1}, false)]
        [TestCase(0, new int[1] {10}, new int[1] {10}, false)]
        [TestCase(0, new int[1] {2}, new int[1] {1}, true)]
        [TestCase(0, new int[1] {2}, new int[1] {10}, true)]
        [TestCase(1, new int[2] {1, 2}, new int[2] {3, 1}, true)]
        [TestCase(1, new int[2] {2, 1}, new int[2] {1, 3}, true)]
        [TestCase(1, new int[2] {2, 1}, new int[2] {1, 30}, true)]
        [TestCase(1, new int[2] {2, 1}, new int[2] {1, 1}, false)]
        [TestCase(3, new int[3] {1, 2, 0}, new int[3] {2, 3, 4}, true)]
        [TestCase(3, new int[3] {1, 2, 0}, new int[3] {4, 6, 8}, true)]
        [TestCase(3, new int[3] {1, 2, 0}, new int[3] {4, 6, 1}, false)]
        [TestCase(3, new int[3] {2, 1, 0}, new int[3] {3, 2, 4}, true)]
        [TestCase(3, new int[3] {2, 1, 0}, new int[3] {3, 1, 4}, false)]
        [TestCase(3, new int[4] {2, 1, 0, 5}, new int[4] {3, 1, 4, 5}, false)]
        public void RecipeConfirmationTest(int BlocksId, int[] items, int[] itemamount, bool ans)
        {
            List<IItemStack> itemStacks = new List<IItemStack>();
            for (int i = 0; i < items.Length; i++)
            {
                itemStacks.Add(new ItemStack(items[i],itemamount[i]));
            }
            var a =MachineRecipeConfig.GetRecipeData(BlocksId, itemStacks).RecipeConfirmation(itemStacks);
            Assert.AreEqual(ans,a);
        }
    }
}