ICAIO-AIO Patch: DO NOT ADD TO SYNTHEIS.ESP!!!!

CAUTION: This patcher is untested outside of my own game. Use it at your own risk, and please send me a bug report if you encounter are any issues.

This patcher is my attempt to patch Immersive Citizens AI Overhaul (ICAIO), with AI Overhaul (AIO). It requires manual patching in xEdit to properly integrate these two mods into your load order, and should only be attempted by experienced modders. To aid in patching, I suggest using modgroups in xEdit to eliminate intentional conflicts caused by this patch.

ICAIO generally uses quests to distribute its AI Packages through quests. These quests distribute specific AI Packages for specific NPCs as well as having several "slots" for other NPCs that may be in the same area. AIO on the other hand directly edits the AI in the NPC records. This leads to the potential that these two mods may "fight" over the NPCs.

I feel like ICAIO is better than AIO in locations where it was fully implemented. However, AIO has more coverage. This patch lets ICAIO be in control of locations where it fully fleshed out the NPCs, while also letting AIO take over in places ICAIO isn't really doing anything.

Here's what I did:
-All ICAIO Quest Aliases in Green Locations (per the ICAIO webpage) were left under ICAIO control. If AIO touched those NPCs, their AI was reverted to vanilla/ussep.
-All ICAIO Quest Aliases with more than 3 AI packages were left under ICAIO Control. If AIO touched those NPCs, their AI was reverted to vanilla/ussep.
-All AI Packages were cleared in aliases with less than 3 AI packages if that NPC was edited in AIO.
-If an npc was in AIO, but not in any ICAIO Quest Alias, that NPC was given the ICAIO Exclusion Faction, so it's changes would not be picked up.

This requires the regular version of AIO. (Not the scripted version, and not the SPID version). There are two ways to correctly implement this mod. Both ways require MANUAL patching of your load order.

METHOD #1: Load the ICAIO_AIO Patch early in your modlist. In this case, add my patcher as a separate group in Synthesis, and run it with ONLY the following mods in the below order:

Base Skyrim

USSEP

AIO

ICAIO


After the patcher finishes, add the ICAIO_AIO patch directly after the two AI mods. DO NOT ADD ANY AIO PATCHER INTO SYNTHESIS!!!! IT WILL UNDO THIS PATCHES CHANGES. Please note that you may need both AIO and ICAIO patches.

Base Skyrim

USSEP

...

...

AIO

ICAIO

ICAIO_AIO PATCH

...

...

SYNTHEIS


METHOD #2: Load ICAIO directly after AIO wherever you would normally in your modlist. Add the correct patches for both mods. Then run Synthesis with all of your normal patches. Then run my patch in a separate group AFTER synthesis. AGAIN, DO NOT ADD ANY AIO PATCHER INTO SYNTHESIS!!!! Load order is as follows:

Base Skyrim

USSEP

...

...

AIO

ICAIO

...

...

SYNTHEIS

ICAIO_AIO PATCH


After running the patcher. Look through the changes for AI packages that may need to be forwarded from mods other than AIO. This patcher reverts the AI packages for ICAIO controlled NPCs back to a vanilla or USSEP state. If AI packages are added by mods other than AIO, then they should be added to the patch manually in xEdit. Again, xEdit modgroups will help reduce the number of conflicts that need to be checked.
