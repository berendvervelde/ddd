
using System.IO;

public class GameProgress {
	public int gold;
	public int steps;
	public int health;
	public int baseHealth;
	public int maxHealth;
	public int gems;
	public int level;
	public int scoreMultiplier;
	public int playerChoice;

	public byte[] Serialize() {
      using (MemoryStream m = new MemoryStream()) {
         using (BinaryWriter writer = new BinaryWriter(m)) {
            writer.Write(gold);
            writer.Write(steps);
			writer.Write(health);
			writer.Write(gems);
			writer.Write(level);
			writer.Write(scoreMultiplier);
			writer.Write(playerChoice);
         }
         return m.ToArray();
      }
   }

   public static GameProgress Deserialize(byte[] data) {
      GameProgress result = new GameProgress();
      using (MemoryStream m = new MemoryStream(data)) {
         using (BinaryReader reader = new BinaryReader(m)) {
            result.gold = reader.ReadInt32();
			result.steps = reader.ReadInt32();
			result.health = reader.ReadInt32();
			result.gems = reader.ReadInt32();
			result.level = reader.ReadInt32();
			result.scoreMultiplier = reader.ReadInt32();
			result.playerChoice = reader.ReadInt32();
         }
      }
      return result;
   }
}
