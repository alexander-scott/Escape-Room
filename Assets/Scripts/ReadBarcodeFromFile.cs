using UnityEngine;
using System.Collections;
using ZXing;

public static class ReadBarcodeFromFile
{
    public static string ReadTexture(WebCamTexture inputTexture)
    {
        // create a barcode reader instance
        IBarcodeReader reader = new BarcodeReader();

        // get texture Color32 array
        var barcodeBitmap = inputTexture.GetPixels32();

        // detect and decode the barcode inside the Color32 array
        var result = reader.Decode(barcodeBitmap, inputTexture.width, inputTexture.height);

        // do something with the result
        if (result != null)
        {
            return result.Text;
        }
        else
        {
            return null;
        }
    }
}