using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using ICAIO_AI_FOR_MOD_NPCs.Utilities;
using Noggog;
using Mutagen.Bethesda.Plugins.Assets;
using OneOf.Types;
using Mutagen.Bethesda.Plugins.Implicit;
using NexusMods.Paths.Trees.Traits;

namespace ICAIO_AIO_Patch
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "ICAIO_AIO_Patch.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var ICAIO = state.LoadOrder.GetModByFileName("Immersive Citizens - AI Overhaul.esp");
            if (ICAIO == null) 
            {
                System.Console.WriteLine("ICAIO Not Found");
                return;
            }

            var AIO = state.LoadOrder.GetModByFileName("AI Overhaul.esp");
            if (AIO == null)
            {
                System.Console.WriteLine("AIO Not Found");
                return;
            }

            var USSEP_Loaded = true;

            var USSEP = state.LoadOrder.GetModByFileName("Unofficial Skyrim Special Edition Patch.esp");
            if (USSEP == null)
            {
                USSEP_Loaded = false;
                System.Console.WriteLine("USSEP Not Found");
                return;
            }

            var DragonbornESM = state.LoadOrder.GetModByFileName("Dragonborn.esm");
            if (DragonbornESM == null)
            {
                System.Console.WriteLine("DragonbornESM Not Found");
                return;
            }

            var HearthfiresESM = state.LoadOrder.GetModByFileName("HearthFires.esm");
            if (HearthfiresESM == null)
            {
                System.Console.WriteLine("HearthfiresESM Not Found");
                return;
            }


            var DawnguardESM = state.LoadOrder.GetModByFileName("Dawnguard.esm");
            if (DawnguardESM == null)
            {
                System.Console.WriteLine("DawnguardESM Not Found");
                return;
            }

            var UpdateESM = state.LoadOrder.GetModByFileName("Update.esm");
            if (UpdateESM == null)
            {
                System.Console.WriteLine("UpdateESM Not Found");
                return;
            }

            var SkyrimESM = state.LoadOrder.GetModByFileName("Skyrim.esm");
            if (SkyrimESM == null)
            {
                System.Console.WriteLine("SkyrimESM Not Found");
                return;
            }

            var packageThreshold = 3;

            var ICAIO_NPC_Alias = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs in ICAIO Aliases
            var AIO_EditedNPCs = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that AIO Edits
            var USSEP_EditedNPCs = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that USSEP Edits
            var DragonbornNPCs = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that Dragonborn DLC Edits
            var HearthFiresNPCs = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that HearthFires DLC Edits
            var DawnguardNPCs = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that Dawnguard DLC Edits
            var UpdateNPCs = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that Update DLC Edits
            var SkyrimNPCs = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that base Skyrim Edits

            var AIO_NPCs_IgnoreFaction = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that AIO edits, but ICAIO does not have an alias for. These will get the ICAIO Exclusion Faction
            var AIO_NPCs_Revert = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that should be controlled by ICAIO. These will have their NPC AI Packages Reverted
            var ICAIO_NPCS_ClearAI  = new HashSet<IFormLinkGetter<INpcGetter>> { }; //NPCs that are in ICAIO aliases that should be controlled by AIO. The quest alias AI Packages will be cleared.
            var ICAIO_EditQuestList =  new HashSet<IFormLinkGetter<IQuestGetter>>(); //List of quests in ICAIO that have NPCs Aliases that need editing.

            //TODO
            // NPCS not touched by AIO, don't delete package data
            // NPCs touched by AIO, but not in an alias, give excusion faction
            // Blacklist Green Cities

            //**************** GREEN ICAIO Quests where AI has been fully implemented (per ICAIO Webpage) *****************

            //Falkreath
            //+Half-Moon Mill
            var NPCO_AIFalkreath = new FormLink<IQuestGetter>(FormKey.Factory("2117E1:Immersive Citizens - AI Overhaul.esp"));
            var NPCO_AIFalkreathJarlMerchant = new FormLink<IQuestGetter>(FormKey.Factory("ECEDE1:Immersive Citizens - AI Overhaul.esp"));

            //Whiterun
            //+Pelagia Farm
            //+Loreius Farm
            //+Chillfurrow Farm
            //+Battle-Born Farm
            //+Honningbrew Meadery
            var NPCO_AIWhiterun = new FormLink<IQuestGetter>(FormKey.Factory("20A698:Immersive Citizens - AI Overhaul.esp"));
            var NPCO_AIWhiterunJarlMerchant = new FormLink<IQuestGetter>(FormKey.Factory("ECEDDC:Immersive Citizens - AI Overhaul.esp"));

            //Riverwood
            var NPCO_AIRiverwood = new FormLink<IQuestGetter>(FormKey.Factory("20A699:Immersive Citizens - AI Overhaul.esp"));

            //Rorikstead
            var NPCO_AIRorikstead = new FormLink<IQuestGetter>(FormKey.Factory("211D4C:Immersive Citizens - AI Overhaul.esp"));

            //Darkwater Crossing
            var NPCO_AIDarkwaterCrossing = new FormLink<IQuestGetter>(FormKey.Factory("2122B9:Immersive Citizens - AI Overhaul.esp"));

            //Windhelm (Light Green)
            var NPCO_AIWindhelm = new FormLink<IQuestGetter>(FormKey.Factory("214E1E:Immersive Citizens - AI Overhaul.esp"));

            //Jorrvaskr
            var NPCO_AICompanionNPCs = new FormLink<IQuestGetter>(FormKey.Factory("9D7E00:Immersive Citizens - AI Overhaul.esp"));

            //Khajiit Caravan Camps
            var NPCO_AIKhajiitCaravansNPCs = new FormLink<IQuestGetter>(FormKey.Factory("B9A81D:Immersive Citizens - AI Overhaul.esp"));

            //ICAIO Tracking Quest
            var NPCO_Trackingystem = new FormLink<IQuestGetter>(FormKey.Factory("022FBA:Immersive Citizens - AI Overhaul.esp"));


            // ICAIO Exclusion Faction to tell ICAIO to ignore this NPC
            var ICAIOExclusionFaction = new FormLink<IFactionGetter>(FormKey.Factory("237FB4:Immersive Citizens - AI Overhaul.esp"));


            //Quest Blacklist used to find Aliases for "Green" locations that should be kept under ICAIO control
            var Quest_Blacklist = new HashSet<IFormLinkGetter<IQuestGetter>>
            {
                NPCO_AIFalkreath,
                NPCO_AIFalkreathJarlMerchant,
                NPCO_AIWhiterun,
                NPCO_AIWhiterunJarlMerchant,
                NPCO_AIRiverwood,
                NPCO_AIRorikstead,
                NPCO_AIDarkwaterCrossing,
                NPCO_AIWindhelm,
                NPCO_AICompanionNPCs,
                NPCO_AIKhajiitCaravansNPCs,
                NPCO_Trackingystem
            };


            //NPC Blacklist for NPCs that ICAIO has covered fully (i.e. green on the webpage), but are included in a quest that may have NPCs we want to edit
            var NPC_Blacklist = new HashSet<IFormLinkGetter<INpcGetter>> 
            { 
               //Whistling Mine NPCs
                Skyrim.Npc.Thorgar,
                Skyrim.Npc.Gunding,
                Skyrim.Npc.Angvid,
                Skyrim.Npc.Badnir
            };

            foreach (var npc in AIO.Npcs)
            {
                AIO_EditedNPCs.Add(npc.ToLink()); //Generate a list of NPCs that AIO touches
            }

            if (USSEP_Loaded)
            {
                
                foreach (var npc in USSEP.Npcs)
                {
                    USSEP_EditedNPCs.Add(npc.ToLink());
                }
            }

            foreach (var npc in DragonbornESM.Npcs)
            {
                DragonbornNPCs.Add(npc.ToLink()); //Generate a list of NPCs that Dragonborn touches
            }

            foreach (var npc in HearthfiresESM.Npcs)
            {
                HearthFiresNPCs.Add(npc.ToLink()); //Generate a list of NPCs that Hearthfires touches
            }

            foreach (var npc in DawnguardESM.Npcs)
            {
                DawnguardNPCs.Add(npc.ToLink()); //Generate a list of NPCs that Dawnguard touches
            }

            foreach (var npc in UpdateESM.Npcs)
            {
                UpdateNPCs.Add(npc.ToLink()); //Generate a list of NPCs that Update touches
            }

            foreach (var npc in SkyrimESM.Npcs)
            {
                SkyrimNPCs.Add(npc.ToLink()); //Generate a list of NPCs that base skyrim touches
            }

            HashSet<ModKey> vanilla = Implicits.Get(GameRelease.SkyrimSE).BaseMasters.ToHashSet(); //List of Vanilla records


            foreach (var quest in ICAIO.Quests) //Loop through each of ICAIO's quests
            {

                if (!vanilla.Contains(quest.FormKey.ModKey)) //Ignore vanilla quests
                {
                    
                    System.Console.WriteLine($"Quest {quest.EditorID} found");

                    if (quest.Aliases.Any()) //Check for quest aliases
                    {

                        System.Console.WriteLine($"Quest has an alias");
                        
                        foreach (var alias in quest.Aliases) //Loop through each quest alias
                        {

                            if (!alias.UniqueActor.IsNull) //If the alias references an NPC and is not null
                            {
                                if (!ICAIO_NPC_Alias.Contains(alias.UniqueActor)) //Add the NPC to the list of NPCs that ICAIO touches
                                {
                                    ICAIO_NPC_Alias.Add(alias.UniqueActor);
                                    System.Console.WriteLine($"ICAIO Quest Alias found for NPC {alias.UniqueActor.FormKey} in Quest {quest.EditorID}-{quest.FormKey}");

                                }

                                if (Quest_Blacklist.Contains(quest))
                                {
                                    if (!AIO_NPCs_Revert.Contains(alias.UniqueActor))
                                    {

                                        AIO_NPCs_Revert.Add(alias.UniqueActor);
                                        System.Console.WriteLine($"NPC {alias.UniqueActor} Should be removed from AIO");
                                    }
                                }

                                if (NPC_Blacklist.Contains(alias.UniqueActor))
                                {
                                    if (!AIO_NPCs_Revert.Contains(alias.UniqueActor))
                                    {

                                        AIO_NPCs_Revert.Add(alias.UniqueActor);
                                        System.Console.WriteLine($"NPC {alias.UniqueActor} Should be removed from AIO");
                                    }

                                }

                                if (alias.PackageData.Count >= packageThreshold)
                                {

                                    if (!AIO_NPCs_Revert.Contains(alias.UniqueActor))
                                    {

                                        AIO_NPCs_Revert.Add(alias.UniqueActor);
                                        System.Console.WriteLine($"NPC {alias.UniqueActor} Should be removed from AIO");
                                    }

                                }


                            }
                        }
                    }
                    
                }

            }

            foreach (var npc in ICAIO_NPC_Alias)
            {
                if (AIO_EditedNPCs.Contains(npc))
                {
                    if (!AIO_NPCs_Revert.Contains(npc))
                    {
                        ICAIO_NPCS_ClearAI.Add(npc);
                        System.Console.WriteLine($"NPC {npc} Should have their AI in ICAIO Cleared");
                    }
                }
            }

            foreach (var npc in AIO.Npcs)
            {
                if (!ICAIO_NPC_Alias.Contains(npc))
                {
                    AIO_NPCs_IgnoreFaction.Add(npc.ToLink());
                    System.Console.WriteLine($"NPC {npc} Should be Ignored by ICAIO");
                }

            }

            foreach (var quest in ICAIO.Quests)
            {

                if (!vanilla.Contains(quest.FormKey.ModKey))
                {

                    if (quest.Aliases.Any())
                    {

                        foreach (var alias in quest.Aliases)
                        {

                            if (!alias.UniqueActor.IsNull)
                            {
                                if (ICAIO_NPCS_ClearAI.Contains(alias.UniqueActor))
                                {
                                    if (!ICAIO_EditQuestList.Contains(quest))
                                    {
                                        ICAIO_EditQuestList.Add(quest.ToLink());
                                        System.Console.WriteLine($"ICAIO Quest {quest.EditorID} Will be edited");
                                    }

                                }

                            }
                        }
                    }

                }

            }

            foreach (var npc in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                if (AIO_NPCs_IgnoreFaction.Contains(npc.ToLink()))
                {
                    var npcOverride = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
                    npcOverride.Factions.Add(
                        new RankPlacement()
                        {
                            Faction = ICAIOExclusionFaction,
                            Rank = 0
                        });

                }

                if (AIO_NPCs_Revert.Contains(npc.ToLink()))
                {
                    var npcOverride = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
                    npcOverride.Packages.RemoveAll(npcOverride.Packages.Contains);
                    npcOverride.SpectatorOverridePackageList.Clear();
                    npcOverride.CombatOverridePackageList.Clear();
                    npcOverride.ObserveDeadBodyOverridePackageList.Clear();
                   
                    if (USSEP_Loaded)
                    {
                        if (USSEP_EditedNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = USSEP.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (DragonbornNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = DragonbornESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (HearthFiresNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = HearthfiresESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (DawnguardNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = DawnguardESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (UpdateNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = UpdateESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (SkyrimNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = SkyrimESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else
                        {
                            System.Console.WriteLine($"Error {npc.EditorID} was not found");
                        }

                    }
                    else
                    {
                        if (DragonbornNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = DragonbornESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (HearthFiresNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = HearthfiresESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (DawnguardNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = DawnguardESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (UpdateNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = UpdateESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else if (SkyrimNPCs.Contains(npc.ToLink()))
                        {
                            var MasterNPC = SkyrimESM.Npcs.TryGetValue<INpcGetter>(npc.FormKey);

                            //npcOverride.Packages.Clear();
                            foreach (var package in MasterNPC.Packages)
                            {
                                npcOverride.Packages.Add(package);
                            }

                            if (MasterNPC.SpectatorOverridePackageList != null)
                            {
                                npcOverride.SpectatorOverridePackageList.SetTo(MasterNPC.SpectatorOverridePackageList);
                            }

                            if (MasterNPC.CombatOverridePackageList != null)
                            {
                                npcOverride.CombatOverridePackageList.SetTo(MasterNPC.CombatOverridePackageList);
                            }

                            if (MasterNPC.ObserveDeadBodyOverridePackageList != null)
                            {
                                npcOverride.ObserveDeadBodyOverridePackageList.SetTo(MasterNPC.ObserveDeadBodyOverridePackageList);
                            }

                        }
                        else
                        {
                            System.Console.WriteLine($"Error {npc.EditorID} was not found");
                        }

                    }


                }


            }

            foreach (var quest in state.LoadOrder.PriorityOrder.Quest().WinningOverrides())
            {
                if (ICAIO_EditQuestList.Contains(quest.ToLink()))
                {
                    var questOverride = state.PatchMod.Quests.GetOrAddAsOverride(quest);

                    foreach (var alias in questOverride.Aliases)
                    {
                        if (!alias.UniqueActor.IsNull)
                        {
                            if (ICAIO_NPCS_ClearAI.Contains(alias.UniqueActor))
                            {
                                System.Console.WriteLine($"quest {quest.EditorID} has NPC {alias.UniqueActor} with {alias.PackageData.Count} packages");
                                if (alias.PackageData.Count > 0)
                                {
                                    alias.PackageData.RemoveAll(alias.PackageData.Contains);    

                                }

                                if (alias.SpectatorOverridePackageList != null)
                                {
                                    alias.SpectatorOverridePackageList.Clear();
                                }

                                if (alias.CombatOverridePackageList != null)
                                {
                                    alias.CombatOverridePackageList.Clear();
                                }

                                if (alias.ObserveDeadBodyOverridePackageList != null)
                                {
                                    alias.ObserveDeadBodyOverridePackageList.Clear();
                                }
                            }
                        }
                    }
                }

            }

        }
    }
}
