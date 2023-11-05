import React from 'react'

const ContextMenuSection = (props) => {
    const {
        children
    } = props;

    return (<>
        <div className='py-4'>
            {children}
        </div>
    </>
    )
}

export default ContextMenuSection