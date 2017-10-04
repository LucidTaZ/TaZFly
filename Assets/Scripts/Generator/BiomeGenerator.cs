using UnityEngine;

public class BiomeGenerator : MonoBehaviour, IBiome {
	public GameObject HumidityNoise;
	public GameObject TemperatureNoise;
	public GameObject ElevationNoise;

	[Range(0.0f, 1.0f)]
	[Tooltip("Weight of elevation factor when determining humidity. Higher = drier.")]
	public float ElevationInfluenceOnHumidity = 0.5f;

	[Range(0.0f, 1.0f)]
	[Tooltip("Weight of elevation factor when determining temperature. Higher = colder.")]
	public float ElevationInfluenceOnTemperature = 0.5f;

	public Texture2D GroundColorMap; // x: temperature, y: precipitation

	bool initialized;

	INoise2D humidityNoise;
	INoise2D temperatureNoise;
	INoise2D elevationNoise;

	public void Awake () {
		Initialize();
	}

	public void Initialize () {
		if (initialized) {
			return;
		}

		humidityNoise = HumidityNoise.GetComponent<INoise2D>();
		temperatureNoise = TemperatureNoise.GetComponent<INoise2D>();
		elevationNoise = ElevationNoise.GetComponent<INoise2D>();
		if (humidityNoise == null || temperatureNoise == null || elevationNoise == null) {
			Debug.LogError("Noise gameobject references contain no INoise2D component.");
		}

		initialized = true;
	}

	public Color GetGroundColor (Vector2 position) {
		float humidity = humidityNoise.Sample(position) * (1 - elevationNoise.Sample(position) * ElevationInfluenceOnHumidity);
		float temperature = temperatureNoise.Sample(position) * (1 - elevationNoise.Sample(position) * ElevationInfluenceOnTemperature);

		Color groundColor = GroundColorMap.GetPixelBilinear(temperature, humidity);

		return groundColor;
	}

	public float GetElevation (Vector2 position) {
		return elevationNoise.Sample(position);
	}
}
