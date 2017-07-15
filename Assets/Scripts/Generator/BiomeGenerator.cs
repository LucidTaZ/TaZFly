using UnityEngine;

public class BiomeGenerator : MonoBehaviour, IBiome {
	public GameObject HumidityNoise;
	public GameObject TemperatureNoise;
	public GameObject HillynessNoise;

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

		// TODO: Improve this extremely simple system
		Color humidityColor = Color.Lerp(Color.yellow, Color.green, humidity);
		Color temperatureColor = Color.Lerp(Color.blue, Color.red, temperature);
		Color groundColor = Color.Lerp(humidityColor, temperatureColor, 0.5f);

		return groundColor;
	}

	public float GetHillyness (Vector2 position) {
		return hillynessNoise.Sample(position);
	}
}
