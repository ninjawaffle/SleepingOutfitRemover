using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;

namespace SleepingOutfitRemover
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SleepingOutfitRemover.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var processed = 0;
            var edited = 0;

            // Loop over all the winning NPCs in the load order
            foreach (var npcGetter in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                processed++;

                // Skip if Sleeping Outfit is already null OR
                // Skip if Inventory template flag is set
                if (
                    npcGetter.SleepingOutfit.IsNull || 
                    npcGetter.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Inventory)
                ) continue;

                // Add NPC to the patch
                var npc = state.PatchMod.Npcs.GetOrAddAsOverride(npcGetter);

                // Set NPC Sleeping Outfit to null
                npc.SleepingOutfit.SetToNull();

                edited++;
            }

            Console.WriteLine($"[SleepingOutfitRemover] Finished processing {processed} records");
            Console.WriteLine($"[SleepingOutfitRemover] {edited} records edited");
        }
    }
}
