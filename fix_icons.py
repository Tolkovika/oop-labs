from PIL import Image
import os

def remove_background(image_path, output_path=None):
    """Remove gray/checkered background from images"""
    try:
        img = Image.open(image_path).convert("RGBA")
        datas = img.getdata()
        
        newData = []
        for item in datas:
            r, g, b, a = item
            # Remove gray backgrounds (checkered pattern colors ~128-140)
            if 120 < r < 150 and 120 < g < 150 and 120 < b < 150:
                newData.append((r, g, b, 0))
            # Remove white-ish backgrounds
            elif r > 240 and g > 240 and b > 240:
                newData.append((r, g, b, 0))
            # Remove light gray
            elif r > 200 and g > 200 and b > 200 and abs(r-g) < 10 and abs(g-b) < 10:
                newData.append((r, g, b, 0))
            else:
                newData.append(item)
        
        img.putdata(newData)
        out = output_path or image_path
        img.save(out, "PNG")
        print(f"Processed: {image_path}")
    except Exception as e:
        print(f"Error: {e}")

# Process problematic icons
files = [
    "SimWeb/wwwroot/images/ostrich.png",
    "SimWeb/wwwroot/images/eagle.png",
    "SimWeb/wwwroot/images/rabbit.png"
]

for f in files:
    if os.path.exists(f):
        remove_background(f)
    else:
        print(f"Not found: {f}")
