# Image Orientate

This is a simple class that physically rotates an image to the orientation defined by its meta tag, and then removes that tag.

It came about after I discovered some browsers do not read this tag (IE) while others do (Chrome, FF etc.), so images appeared differently which is a fairly major problem.

This class can be used as part of the uploading process of images in a .NET application, or when the files are served.