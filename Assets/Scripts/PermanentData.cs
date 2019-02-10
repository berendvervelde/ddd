using System.IO;
public class PermanentData {

    public int highscore;           // best score ever
	public int gold;                // available gold to buy permanent stuff
	public int knightBaseHealth;
	public int wizardBaseHealth;
	public int rogueBaseHealth;
	public int rangerBaseHealth;
	public int dwarfBaseHealth;
    public int knightStrength;
	public int wizardStrength;
	public int rogueStrength;
	public int rangerStrength;
	public int dwarfStrength;

    public byte[] Serialize() {
        using (MemoryStream m = new MemoryStream()) {
            using (BinaryWriter writer = new BinaryWriter(m)) {
                writer.Write(highscore);
				writer.Write(gold);
				writer.Write(knightBaseHealth);
				writer.Write(wizardBaseHealth);
				writer.Write(rogueBaseHealth);
				writer.Write(rangerBaseHealth);
				writer.Write(dwarfBaseHealth);
                writer.Write(knightStrength);
                writer.Write(wizardStrength);
                writer.Write(rogueStrength);
                writer.Write(rangerStrength);
                writer.Write(dwarfStrength);
            }
            return m.ToArray();
        }
    }
    public static PermanentData Deserialize(byte[] data) {
        PermanentData result = new PermanentData();
        using (MemoryStream m = new MemoryStream(data)) {
            using (BinaryReader reader = new BinaryReader(m)) {
                result.highscore = reader.ReadInt32();
				result.gold = reader.ReadInt32();
				result.knightBaseHealth = reader.ReadInt32();
				result.wizardBaseHealth = reader.ReadInt32();
				result.rogueBaseHealth = reader.ReadInt32();
				result.rangerBaseHealth = reader.ReadInt32();
				result.dwarfBaseHealth = reader.ReadInt32();
                result.knightStrength = reader.ReadInt32();
                result.wizardStrength = reader.ReadInt32();
                result.rogueStrength = reader.ReadInt32();
                result.rangerStrength = reader.ReadInt32();
                result.dwarfStrength = reader.ReadInt32();
            }
        }
        return result;
    }
}
