using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.IO;
using TravelPlanner;

namespace TravelPlanner 
{
    /// <summary>
    /// Loads travel planning instructions from SKILL.md.
    ///
    /// The model can call this function whenever it needs
    /// detailed travel-planning behavior and rules.
    /// </summary>
    public class TravelSkills
    {
        [KernelFunction("load_skill")]
        [Description(
            "Loads the Travel Planner skill instructions. Call this before generating a travel itinerary or when detailed travel planning guidance is required."
        )]
        public string LoadSkill(
            [Description(
                "Skill name. Use travel-planner."
            )]
            string skillName)
        {
            Console.WriteLine(
                $"   [Skill Loaded] {skillName}");

            string skillFile =
                Path.Combine(
                    AppConfig.SkillsFolder,
                    Path.GetFileName(skillName),
                    "SKILL.md");

            if (!File.Exists(skillFile))
            {
                return $"Skill not found: {skillName}";
            }

            return RemoveFrontMatter(
                File.ReadAllText(skillFile));
        }

        /// <summary>
        /// Removes YAML front matter and returns only
        /// the skill instructions.
        /// </summary>
        private static string RemoveFrontMatter(
            string text)
        {
            int start =
                text.IndexOf("---");

            int end =
                text.IndexOf(
                    "---",
                    start + 3);

            if (start < 0 || end < 0)
            {
                return text;
            }

            return text
                .Substring(end + 3)
                .Trim();
        }
    }
}