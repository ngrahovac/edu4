import React from 'react'
import HatParam from './HatParam'

const AcademicHat = ({ hat }) => {
    return (
        <div className='flex gap-x-2 gap-y-1 flex-wrap'>
            <HatParam>Academic / Researcher</HatParam>
            <HatParam>{hat.parameters.researchField}</HatParam>
        </div>
    )
}

export default AcademicHat