from PIL import Image
import os
import glob

def remove_white_background(image_path):
    try:
        img = Image.open(image_path).convert("RGBA")
        datas = img.getdata()

        newData = []
        for item in datas:
            # Check if pixel is white-ish (tolerance of 10)
            if item[0] > 245 and item[1] > 245 and item[2] > 245:
                newData.append((255, 255, 255, 0))
            else:
                newData.append(item)

        img.putdata(newData)
        img.save(image_path, "PNG")
        print(f"Processed {image_path}")
    except Exception as e:
        print(f"Error processing {image_path}: {e}")

# Process specific unit files
files = [
    "SimWeb/wwwroot/images/elf.png",
    "SimWeb/wwwroot/images/orc.png",
    "SimWeb/wwwroot/images/bird.png",
    "SimWeb/wwwroot/images/rabbit.png",
    "SimWeb/wwwroot/images/elf_transparent.png",
    "SimWeb/wwwroot/images/orc_transparent.png"
]

for f in files:
    if os.path.exists(f):
        remove_white_background(f)
