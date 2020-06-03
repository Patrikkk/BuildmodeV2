using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Buildmode
{
    /// <summary>
    /// Handles incoming packet data.
    /// </summary>
    class GetPacketHandler
    {
        /// <summary>
        /// Registers packet event hooks.
        /// </summary>
        public static void InitGetPacketHandler()
        {
            GetDataHandlers.TogglePvp.Register(OnTogglePvp);
            GetDataHandlers.PlayerHP.Register(OnPlayerHP);
            GetDataHandlers.PlayerBuffUpdate.Register(OnBuffUpdate);
            GetDataHandlers.Teleport.Register(OnTeleport);
            GetDataHandlers.PaintTile.Register(OnPaintTile);
            GetDataHandlers.PaintWall.Register(OnPaintWall);
            GetDataHandlers.TileEdit.Register(OnTileEdit);
            GetDataHandlers.PlayerDamage.Register(OnPlayerDamage);
        }
        /// <summary>
        /// Invoked when a player toggles PVP. 
        /// If their PVP is on while their buildmode is enabled, buildmode is turned off and the player is notified.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTogglePvp(object sender, TogglePvpEventArgs e)
        {
            if (e.Pvp && e.Player.IsBuildModeOn())
            {
                e.Player.ToggleBuildMode();
                e.Player.SendInfoMessage($"You are in PvP mode. Buildmode has been disabled!");
            }
        }
        /// <summary>
        /// Invoked when the health of the player is modified.
        /// If their health is not full while their buildmode is on, they get healed to full health.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPlayerHP(object sender, PlayerHPEventArgs e)
        {
            if ((e.Current < e.Max) && e.Player.IsBuildModeOn())
            {
                e.Player.Heal(e.Player.TPlayer.statLifeMax2);
            }
        }
        /// <summary>
        /// Invoked when a buff value of the player is modified.
        /// If their buildmode is on, we set buffs of PluginUtils.BuildModeBuffs on the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnBuffUpdate(object sender, PlayerBuffUpdateEventArgs e)
        {
            if (e.Player.IsBuildModeOn())
            {
                foreach (int buffID in PluginUtils.BuildModeBuffs)
                    e.Player.SetBuff(buffID, Int16.MaxValue);
            }
        }
        /// <summary>
        /// Invoked whenever the player teleports.
        /// If their buildmode is on and they have ChaosState from Rod of Discord, heal the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTeleport(object sender, TeleportEventArgs e)
        {
            if (e.Player.IsBuildModeOn())
            {
                if (e.Player.TPlayer.FindBuffIndex(BuffID.ChaosState) > -1)
                    e.Player.Heal(100);
            }
        }
        /// <summary>
        /// Invoked whenever the player paints a tile.
        /// If their buildmode is on and they are low on the last used paint, they get a full stack.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPaintTile(object sender, PaintTileEventArgs e)
        {
            if (e.Player.IsBuildModeOn())
            {
                int count = 0;
                Item lastItem = null;
                foreach (Item i in e.Player.TPlayer.inventory)
                {
                    if (i.paint == e.type)
                    {
                        lastItem = i;
                        count += i.stack;
                    }
                }
                if (count <= 5 && lastItem != null)
                    e.Player.GiveItemCheck(lastItem.type, lastItem.Name, lastItem.stack);
            }
        }
        /// <summary>
        /// Invoked whenever the player paints a wall.
        /// If their buildmode is on and they are low on the last used paint, they get a full stack.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPaintWall(object sender, PaintWallEventArgs e)
        {
            if (e.Player.IsBuildModeOn())
            {
                int count = 0;
                Item lastItem = null;
                foreach (Item i in e.Player.TPlayer.inventory)
                {
                    if (i.paint == e.type)
                    {
                        lastItem = i;
                        count += i.stack;
                    }
                }
                if (count <= 5 && lastItem != null)
                    e.Player.GiveItemCheck(lastItem.type, lastItem.Name, lastItem.stack);
            }
        }
        /// <summary>
        /// Invoked whenever the player edits a tile.
        /// If their buildmode is on, and they are low on the currently used tile item, they get a full stack.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTileEdit(object sender, TileEditEventArgs e)
        {
            if (e.Player.IsBuildModeOn())
            {
                int count = 0;
                Item lastItem = e.Player.SelectedItem;
                if ((e.Action == EditAction.PlaceTile || e.Action == EditAction.PlaceWall) && e.Player.TPlayer.inventory[e.Player.TPlayer.selectedItem].type != ItemID.StaffofRegrowth)
                {
                    if (e.Player.SelectedItem.tileWand > 0)
                        e.EditData = (short)e.Player.SelectedItem.tileWand;

                    if ((e.Action == EditAction.PlaceTile && e.Player.SelectedItem.createTile == e.EditData) ||
                        (e.Action == EditAction.PlaceWall && e.Player.SelectedItem.createWall == e.EditData))
                    {
                        count += e.Player.SelectedItem.stack;
                    }
                    if (count <= 1 && lastItem != null)
                        e.Player.GiveItemCheck(lastItem.type, lastItem.Name, 999);
                }
                else if (e.Action == EditAction.PlaceWire || e.Action == EditAction.PlaceWire2 || e.Action == EditAction.PlaceWire3 || e.Action == EditAction.PlaceWire4)
                {
                    foreach (Item i in e.Player.TPlayer.inventory)
                    {
                        if (i.type == ItemID.Wire)
                            count += i.stack;
                    }
                    if (count <= 5)
                        e.Player.GiveItemCheck(ItemID.Wire, "Wire", 1000 - count);
                }
                else if (e.Action == EditAction.PlaceActuator)
                {
                    foreach (Item i in e.Player.TPlayer.inventory)
                    {
                        if (i.type == ItemID.Actuator)
                            count += i.stack;
                    }
                    if (count <= 5)
                        e.Player.GiveItemCheck(ItemID.Actuator, "Actuator", 1000 - count);
                }
            }
        }
        /// <summary>
        /// Invoked whenever the player is damaged.
        /// If their buildmode is on, they get healed by the amount of damage they were dealt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPlayerDamage(object sender, PlayerDamageEventArgs e)
        {
           if (e.Player.IsBuildModeOn())
            {
                e.Player.Heal(e.Player.TPlayer.statLifeMax);
            }
        }
    }
}
