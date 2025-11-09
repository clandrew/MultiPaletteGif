# MultiPaletteGif
This is a utility for creating animated GIFs with one frame buffer, multiple palettes, for when you want to do all your image editing in an external editor.

Features:
* Flexible palette size up to 256 colors
* Easy-to-use source format compatible with external editors
* Press 'F5' to refresh the image from disk
* Scrub through frames manually or toggle 'Autoplay' to animate the preview continuously
* Can set the speed of the GIF and loop count, or infinite loop
* Can apply a scale factor to the output

<img src="https://raw.githubusercontent.com/clandrew/MultiPaletteGif/master/Images/SourceImage.PNG" width="300"/>

![Example image](https://raw.githubusercontent.com/clandrew/MultiPaletteGif/master/Images/Screenshot.PNG "Example image")

![Example image](https://raw.githubusercontent.com/clandrew/MultiPaletteGif/master/Images/export.gif "Example image")

The program runs standalone without an installer.

## Supported input formats
For the source image, the program supports all the formats supported by WIC: png, bmp, jpg, etc. It does not support other formats, for example SVG.

Because the program uses hardware bitmaps, images must be under the Direct3D GPU texture limit. Generally, this means having neither width nor height exceeding 16384.

## Release
You don't need to build the code to run it. Download the release from the [releases page](https://github.com/clandrew/MultiPaletteGif/releases).

The program runs on x86-64-based Windows environments. Tested on Windows 10.

## Build
The code is organized as a Visual Studio 2019 solution. The solution consists of two projects.
* MultiPaletteGif - This is a .NET exe, a Windows Forms program written in C#. It has the UI.
* Native - This is a DLL written in C++. It handles image encoding and decoding using WIC.

The solution is built for x86 architecture. Tested on Windows 10 22H2.
