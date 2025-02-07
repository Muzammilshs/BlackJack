// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("0LtQ2rSr6jojfagHmfyaUko2L4/ZXNvqOcEhvb2t/S5ZrHpLM2ZBXqHimYhAhkBCFzuoxc3LDFVzc/sQEs+neFvi4Vk3vJt2lkIAobLqCUyITYiwEQa83BfX6+mV7uKJAOdnGGBq+VkX7XL4QStWHIHl5Wl3VnCWri0jLByuLSYuri0tLK6zp4W2bXEcri0OHCEqJQaqZKrbIS0tLSksL0jT+Wk8Yfw7z6PwDsiYoh2MrJNTnJFjIuSmejLmQRfolXP8z5Sw+toUhnj7DeqMetrd2znmhb7lvW6cJd6HafLPkepSglgpEKqCguVy+JQMpbnvMlRFscEQF0DIsINLDQYfj8jkRote2tudmpmaVNVBrpjsxSFoEfVzYYlwmhV8nS4vLSwt");
        private static int[] order = new int[] { 13,9,2,7,12,11,9,13,12,9,11,11,13,13,14 };
        private static int key = 44;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
