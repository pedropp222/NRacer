using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FadeCamera : MonoBehaviour
{
	[Range (0f, 1f)]
	public float opacity = 1;
	public Color color = Color.black;

	private Material material;
	private float startTime = 0;
	private float startOpacity = 1;
	private int endOpacity = 1;
	private float duration = 0;
	public bool isFading = false;

	public Status current;

	public void FadeIn (float duration = 1)
	{
		this.duration = duration;
		this.startTime = Time.time;
		this.startOpacity = opacity;
		this.endOpacity = 1;
		this.isFading = true;
		current = Status.FADING_IN;
	}

	public void FadeOut (float duration = 1)
	{
		this.duration = duration;
		this.startTime = Time.time;
		this.startOpacity = opacity;
		this.endOpacity = 0;
		this.isFading = true;
		current = Status.FADING_OUT;
	}

	void Awake ()
	{
		material = new Material (Shader.Find ("Hidden/FadeCameraShader"));
		current = Status.NONE;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (isFading && duration > 0) {
			opacity = Mathf.Lerp (startOpacity, endOpacity, (Time.time - startTime) / duration);
			isFading = opacity != endOpacity;
		}
		else
		{
			current = Status.NONE;
		}

		if (opacity == 1f) {
			Graphics.Blit (source, destination);
			return;
		}

		material.color = color;
		material.SetFloat ("_opacity", opacity);
		Graphics.Blit (source, destination, material);
	}

	public enum Status
	{
		NONE,
		FADING_IN,
		FADING_OUT
	}
}
