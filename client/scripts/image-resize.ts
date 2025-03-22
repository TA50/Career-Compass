import { assert } from "./assertions";
import { info, success } from "./logs";
import sharp from "sharp";

async function main() {
    assert(process.argv.length).isInRange(
        5,
        6,
        "Usage: ts-node image-resize.ts <file-path> <size> <output-path>  <type?> "
    );

    const filePath = process.argv[2];
    const size = process.argv[3];
    assert(size).matches(
        /^\d+-\d+$/,
        "Size must be in the format <width>-<height> in px"
    );
    const width = parseInt(size.split("-")[0]);
    const height = parseInt(size.split("-")[1]);
    const outputPath = process.argv[4];
    const type = process.argv[5] || "webp";
    assert(type).oneOf(
        ["webp", "jpeg", "png"],
        "Type must be one of webp, jpeg, png"
    );

    assert(outputPath.split(".")[1]).is(
        type,
        `Output path must have a ${type} extension`
    );

    const image = sharp(filePath);
    image.resize(width, height);
    info(`Resizing image to ${size} and saving to ${outputPath}`);
    await image.toFile(outputPath);
    success(`Image resized and saved to ${outputPath}`);
}

main();

