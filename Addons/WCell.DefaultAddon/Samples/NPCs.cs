using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCell.Constants.NPCs;
using WCell.Core.Initialization;
using WCell.RealmServer.Entities;
using WCell.RealmServer.Global;
using WCell.RealmServer.NPCs;
using WCell.Constants.World;
using WCell.Util.Graphics;

namespace WCell.Addons.Default.Samples
{
	public static class NPCs
	{
		#region Teleport NPCs
		/// <summary>
		/// 
		/// </summary>
		[Initialization]
		[DependentInitialization(typeof(NPCMgr))]
		public static void SetupTeleportGossips()
		{
			var loc = WorldLocationMgr.Stormwind;
			if (loc != null)
			{
				CreateTeleportNPC(NPCId.ZzarcVul, loc);
			}
		}

		public static void CreateTeleportNPC(NPCId id, IWorldLocation loc)
		{
			CreateTeleportNPC(id, loc.Position, loc.MapId);
		}

		public static void CreateTeleportNPC(uint id, Vector3 location, MapId mapId)
		{
			CreateTeleportNPC((NPCId)id, location, mapId);
		}

		public static void CreateTeleportNPC(NPCId id, Vector3 location, MapId mapId)
		{
			var spawn = new NPCSpawnEntry
			{
				EntryId = id
			};
			spawn.FinalizeDataHolder();

			spawn.Spawned += npc =>
			{
				npc.Invulnerable++;
				npc.GossipMenu = WorldLocationMgr.CreateTeleMenu();
			};
		}
		#endregion
	}
}