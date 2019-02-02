Important: please import the standard "effects" package (camera effects) before importing this package in a project. 
The demo scene requires deferred & linear. Also, when you import the FFS pack, the "Color Correction Lookup" Script
on the camera will have to be fixed. You would need to apply the lookup texture from "Standard Assets > Effects > 
ImageEffects > Textures > ContrastEnhanced3D16.png"  


Shaders in this package:

FastFakeSkin - diffuse + skincap. Based on Unity Standard Shader. Has been tested on android. 

NotSoFastFakeSkin - diffuse + normal + skincap.  Based on Unity Standard Shader. 

NotSoFastNotSoFakeSkin - diffuse + masks + normal + skincap + brdf. Advanced shader. Masks texture is not required for it to work. 

Thank you for downloading!