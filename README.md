## Triptych
Triptych is software for interactive visual art experiments on the theme of triptych displays.

Technologies: XAML (WPF & Silverlight), Webcam, QR code recognition.

### Installations

Dolce Vita (GriikBuffet) at Anti-Design 2014 (Technopolis, Athens)
* https://github.com/Zoomicon/Triptych/wiki/Dolce-Vita
* http://www.technopolis-athens.com/web/guest/events/~/eventsbrowser/15097/29/0/0

### Software

* __Triptych.Demo.Silverlight.WebCam__

    This is a Silverlight-based web application, that shows a Triptych, that is a tripartite 1:2:1-sized display using live video feeds from one or more webcams. 

    The left and right panes of the Triptych have their leftmost and rightmost halves cropped out of the display respectively. Those panes are both using right-to-left (RTL) display option, so that they show the respective video feeds horizontally flipped. If the left and right panes are assigned to show the same content, this results in them showing the left and right halves respectively of the horizontally flipped content.

    Clicking each pane of the Triptych, one can select a different video feed as source for that pane if they have multiple webcams connected.

    Selecting the same video feed for each pane of the triptych creates an interesting kaleidoscope-like effect, since the vertical borders between the triptych panes are practically mirroring axes.

* __Triptych.Demo.WPF.ImageFolders__

    This is a WPF-based Windows desktop application, that shows a Triptych, that is a 1:2:1-sized display using images from a local file folder that are organized in thematic subfolders (e.g. Travel, Sports etc.). There is also a special "Startup" subfolder with images to be shown when the system starts up, until a user first interacts with it.

    The user picks an image subfolder, pointing a respective QR code to a webcam. The QR code can contain the name of the image subfolder, or a URL, the last part of which is the subfolder's name.

    The left and right panes on the Triptych have their leftmost and rightmost halves cropped out of the display respectively. Those panes are both using right-to-left (RTL) display option, so that they show the respective image horizontally flipped. Since they're both assigned to show the same image, this results in them showing the left and right halves respectively of the horizontally flipped image.

    A timer (background thread) picks another image as source for the middle and the left/right panes of the triptych, from a random-ordered list of the images in the currently chosen subfolder. The middle and the left/right images are picked from the same list with a different frequency (time delay), so that different combinations of images (Triptychs) can be achieved as time passes.

    Periodically, the same image may be picked for both the middle an the left/right panes on the triptych, creating an interesting kaleidoscope-like effect, since the vertical borders between the triptych panes are practically mirroring axes. This effect was inspired from the left and right doors of a tripartite buffet furniture with 1:2:1 part ratios, that open to respective directions, splitting a view in half and flipping its parts outwards to the left and right sides.

   __Using the cloud:__

   The images folder and the application can be on Dropbox or other Cloud Storage solution that is synchronized to a local folder so that they are easily updated from a remote location. This also facilitates offline updates, even when the installation computer is shut down, so that it catches up with any updates when the system and local cloud storage synchronization agent is restarted.

   One can have the QR codes contain custom short URLs generated out of shared web links provided by a cloud storage that hosts the image subfolders. For example, Dropbox allows one to right-click a subfolder under its local synchronization folder and select to copy a (pretty long) sharing URL to the system clipboard to paste elsewhere and share. Such short URLs can be constructed using services like TinyUrl.com, but also using link tracking ones like Bit.ly. That way one can count get web analytics (statistical info) for user visits to the URLs hosted by the respective QR codes (e.g. using a smartphone's camera via a QR reader app). Such URLs apart from being used as tokens to point to a local image subfolder, can serve as online web galleries displaying the same images of those folders (e.g. Dropbox sharing URLs point to photo galleries on the web for those cloud-shared folders that contain only images).

