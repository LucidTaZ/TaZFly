using UnityEngine;

public class BiomeGenerator : MonoBehaviour, IBiome {
	public GameObject HumidityNoise;
	public GameObject TemperatureNoise;
	public GameObject HillynessNoise;

	public Texture2D GroundColorMap; // x: temperature, y: precipitation

	bool initialized;

	INoise2D humidityNoise;
	INoise2D temperatureNoise;
	INoise2D hillynessNoise;

	public void Awake () {
		Initialize();
	}

	public void Initialize () {
		if (initialized) {
			return;
		}

		humidityNoise = HumidityNoise.GetComponent<INoise2D>();
		temperatureNoise = TemperatureNoise.GetComponent<INoise2D>();
		hillynessNoise = HillynessNoise.GetComponent<INoise2D>();
		if (humidityNoise == null || temperatureNoise == null || hillynessNoise == null) {
			Debug.LogError("Noise gameobject references contain no INoise2D component.");
		}

		initialized = true;
	}

	public Color GetGroundColor (Vector2 position) {
		float humidity = humidityNoise.Sample(position);
		float temperature = temperatureNoise.Sample(position);

		Color groundColor = GroundColorMap.GetPixelBilinear(temperature, humidity);

		return groundColor;
	}

	public float GetHillyness (Vector2 position) {
		return hillynessNoise.Sample(position);
	}
}
