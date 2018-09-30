﻿using System.Reflection;
using Verse;
using UnityEngine;
using Harmony;
using RimWorld;

namespace Replace_Stuff
{
	public class Mod : Verse.Mod
	{
		public Mod(ModContentPack content) : base(content)
		{
			// initialize settings
			GetSettings<Settings>();
#if DEBUG
			HarmonyInstance.DEBUG = true;
#endif

			//Need to patch this while loading
			HarmonyInstance harmony = HarmonyInstance.Create("Uuugggg.rimworld.Replace_Stuff.main");

			//Turn of DefOf warning since harmony patches trigger it.
			harmony.Patch(AccessTools.Method(typeof(DefOfHelper), "EnsureInitializedInCtor"),
				new HarmonyMethod(typeof(EnsureInitializedInCtor), "Prefix"), null);
			
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		[StaticConstructorOnStartup]
		public static class ModStartup
		{
			static ModStartup()
			{
				//Hugslibs-added defs will be queued up before this Static Constructor
				//So queue replace frame generation after that
				LongEventHandler.QueueLongEvent(() => ThingDefGenerator_ReplaceFrame.AddReplaceFrames(), null, true, null);
			}
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);
			GetSettings<Settings>().DoWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "TD.ReplaceStuff".Translate();
		}
	}
}