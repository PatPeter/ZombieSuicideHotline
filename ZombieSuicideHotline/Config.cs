using System.ComponentModel;
using Exiled.API.Interfaces;
using System.Collections.Generic;

namespace ZombieSuicideHotline
{
    public class Config : IConfig {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

		[Description("How long between each use of Recall?")]
		public float RecallCooldown { get; set; } = 20f;

        [Description("How long between each use of Respawn?")]
        public float RetreatCooldown { get; set; } = 40f;

        [Description("A list of classes that should beable to call the suicide hotline and what precent of their health is done")]
        public Dictionary<string, float> HotlineCalls { get; set; } = new Dictionary<string, float>
        {
			{
				"Scp049", -1f
			},
			{
                "Scp0492", 0f
            },
			{
				"Scp096", -1f
			},
			{
				"Scp106", -1f
			},
			{
				"Scp173", -1f
			},
			{
				"Scp93953", -1f
			},
			{
				"Scp93989", -1f
			},
		};
    }
}
