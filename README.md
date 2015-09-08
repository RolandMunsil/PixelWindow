# PixelWindow
A library for creating windows with the ability to set the color of individual client pixels.

For example:
```csharp
public static void Main(String[] args)
{
    //Note that the 'true' causes the y-axis to be upward instead of the usual downward. 
    //So, for example, (0, 0) is the bottom left instead of the top left.
    using (PixelWindow window = new PixelWindow(1280, 720, true))
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

                Color color = new Color
                {
                    red = whiteness,
                    green = whiteness,
                    blue = whiteness,
                };

                window[x, y] = color;
            }
        }

        //Show the screen.
        window.UpdateClient();

        //Save the image as a PNG.
        window.SaveClientToPNG("Cool Circles.png");

        //Wait for the window to be closed by the user.
        while (window.IsOpen) ;
    }
}
```
This will generate the following image:
![Cool circles!](http://i.imgur.com/79inIJ8.png)

*Also: If you see something in here that looks problematic, please tell me! I am not experienced with SDL and would love to know how I can improve my code!*
