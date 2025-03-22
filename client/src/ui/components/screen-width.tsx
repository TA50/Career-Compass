'use client';
import React, { useState, useEffect } from 'react';
import config from "@/lib/config";
import { getBreakpoint } from '@/lib/breakpoint';

export const ScreenWidth = () => {
    const [width, setWidth] = useState(window.innerWidth);
    const [breakpoint, setBreakpoint] = useState(getBreakpoint(window.innerWidth));

    useEffect(() => {
        const handleResize = () => {
            setWidth(window.innerWidth);
            setBreakpoint(getBreakpoint(window.innerWidth));
        };

        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    if (config.isDevelopment) {
        return (
            <div className='fixed rounded-sm right-1 bottom-1 text-xs font-light  bg-gray-700 text-primary-foreground p-2'>
                {breakpoint}:{width}px
            </div>
        );
    } else {
        return null;
    }
};