﻿using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace FieldsOfGold.Items
{
    class FOGCattailRoot : ItemCattailRoot
    {
        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            if (blockSel == null || byEntity?.World == null || !byEntity.Controls.Sneak)
            {
                base.OnHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handHandling);
                return;
            }

            bool waterBlock = byEntity.World.BlockAccessor.GetBlock(blockSel
                .Position.AddCopy(blockSel.Face)).LiquidCode == "water";
            Block block;

            if (this.Code.Path.Contains("papyrus"))
            {
                block = byEntity.World.GetBlock(new AssetLocation(waterBlock ? "tallplant-papyrus-water-growing-free" : "tallplant-papyrus-land-growing-free"));
            }
            else
            {
                block = byEntity.World.GetBlock(new AssetLocation(waterBlock ? "tallplant-coopersreed-water-growing-free" : "tallplant-coopersreed-land-growing-free"));
            }

            if (block == null)
            {
                base.OnHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handHandling);
                return;
            }

            IPlayer byPlayer = null;
            if (byEntity is EntityPlayer) byPlayer = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

            blockSel = blockSel.Clone();
            blockSel.Position.Add(blockSel.Face);

            string useless = "";

            bool ok = block.TryPlaceBlock(byEntity.World, byPlayer, itemslot.Itemstack, blockSel, ref useless);

            if (ok)
            {
                byEntity.World.PlaySoundAt(block.Sounds.GetBreakSound(byPlayer), blockSel.Position.X + 0.5, blockSel.Position.Y + 0.5, blockSel.Position.Z + 0.5, byPlayer);
                itemslot.TakeOut(1);
                itemslot.MarkDirty();
                handHandling = EnumHandHandling.PreventDefaultAction;
            }
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[] {
                new WorldInteraction()
                {
                    HotKeyCode = "sneak",
                    ActionLangCode = "heldhelp-plant",
                    MouseButton = EnumMouseButton.Right,
                }
            }.Append(base.GetHeldInteractionHelp(inSlot));
        }

    }
}