using UnityEngine;
using UnityEngine.U2D;

public class ForceGenerator : MonoBehaviour

{

    private bool forceGenerateOnce = true;

    void OnGUI()

    {

        var sc = GetComponent<SpriteShapeController>();

        var sr = GetComponent<SpriteShapeRenderer>();

        if (sr != null)

        {

            if (!sr.isVisible && forceGenerateOnce)

            {

                sc.BakeMesh();

                UnityEngine.Rendering.CommandBuffer rc = new UnityEngine.Rendering.CommandBuffer();

                var rt = RenderTexture.GetTemporary(256, 256, 0, RenderTextureFormat.ARGB32);

                Graphics.SetRenderTarget(rt);

                rc.DrawRenderer(sr, sr.sharedMaterial);

                Graphics.ExecuteCommandBuffer(rc);

                Debug.Log("SpriteShape Generated");

                forceGenerateOnce = false;

            }

        }

    }

}

