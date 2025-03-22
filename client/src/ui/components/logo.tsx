const Logo = (props: React.SVGProps<SVGSVGElement>) => {
    return (
        <svg
            viewBox="-3 -3 36.00 36.00"
            version="1.1"
            xmlns="http://www.w3.org/2000/svg"
            xmlnsXlink="http://www.w3.org/1999/xlink"
            fill={props.fill}
            {...props}
        >
            <g id="SVGRepo_bgCarrier" strokeWidth="0"></g>
            <g id="SVGRepo_tracerCarrier" strokeLinecap="round" strokeLinejoin="round"></g>
            <g id="SVGRepo_iconCarrier">
                <title>compass</title>
                <desc>Created with Sketch Beta.</desc>
                <defs></defs>
                <g
                    id="Page-1"
                    stroke="none"
                    strokeWidth="1"
                    fill="none"
                    fillRule="evenodd"
                >
                    <g
                        id="Icon-Set"
                        transform="translate(-465.000000, -360.000000)"
                        fill={props.fill}
                    >
                        <path
                            d="M478.951,373.451 L482.475,370.348 L481.549,374.951 L483.281,375.951 L484.597,366.673 L477.219,372.451 L478.951,373.451 L478.951,373.451 Z M480,388 C472.819,388 467,382.181 467,375 C467,367.82 472.819,362 480,362 C487.181,362 493,367.82 493,375 C493,382.181 487.181,388 480,388 L480,388 Z M480.006,360.012 C471.725,360.012 465.012,366.726 465.012,375.006 C465.012,383.287 471.725,390 480.006,390 C488.287,390 495,383.287 495,375.006 C495,366.726 488.287,360.012 480.006,360.012 L480.006,360.012 Z M481,375.5 C481,374.672 480.328,374 479.5,374 C478.672,374 478,374.672 478,375.5 C478,376.328 478.672,377 479.5,377 C480.328,377 481,376.328 481,375.5 L481,375.5 Z M476.525,380.652 L477.451,376.049 L475.719,375.049 L474.404,384.327 L481.781,378.549 L480.049,377.549 L476.525,380.652 L476.525,380.652 Z"
                            id="compass"
                        ></path>
                    </g>
                </g>
            </g>
        </svg>
    );
};

export default Logo;