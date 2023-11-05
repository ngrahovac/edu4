import React from 'react'

const InvalidFormFieldWarning = (props) => {
    const {
        visible = false,
        text = "The provided value is not valid"
    } = props;

    return (
        <div className='h-8 text-red-500 font-semibold'>
            {
                visible &&
                <>
                    {text}
                </>
            }
        </div>
    )
}

export default InvalidFormFieldWarning