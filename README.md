# PixelWindow
A library for creating windows with the ability to set the color of individual client pixels.

For example:
```csharp
public static void Main(String[] args)
{
    using (PixelWindow window = new PixelWindow(1280, 720, "Circles Example"))
    {
        //Loop through all the pixels
        for (int x = 0; x < window.ClientWidth; x++)
        {
            for (int y = 0; y < window.ClientHeight; y++)
            {
                //Distance from bottom left of screen (0, 0)
                int distance = (int)Math.Sqrt(x * x + y * y);

                //Make the gradient loop every 256 pixels
                byte whiteness = (byte)(distance % 256);

                ARGBColor color = new ARGBColor
                {
                    red = whiteness,
                    green = whiteness,
                    blue = whiteness,
                    reserved = 0
                };

                window[x, y] = color;
            }
        }
        
        //Show the screen.
        window.UpdateClient();

        //Save the image as a PNG.
        window.BackBuffer.Save("Cool Circles.png");

        //Wait for the window to be closed by the user.
        while (!window.IsClosed) ;
    }
}
```
This will generate the following image:
![Cool circles!](http://i.imgur.com/79inIJ8.png)

Note that I am mostly just making this for myself, so I make no guarantees about how fast it is or whether or not it will leak tons of resources or anything like that.
