import { PropsWithChildren } from 'react';

export default function Page(props: PropsWithChildren) {
    return (
        <main
            className='p-4 lg:px-8'
        >{props.children}</main>
    )
}
