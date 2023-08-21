import React from 'react'

const Nudge = (props) => {
    const {
        problem,
        solution
    } = props;

    return (
        <div className='w-96 text-justify'>
            {problem}
            <br />
            <p className='font-bold mt-2'>
                {solution}
            </p>
        </div>
    )
}

export default Nudge