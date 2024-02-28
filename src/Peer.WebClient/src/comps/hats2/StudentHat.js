import React from 'react'
import HatParam from './HatParam';

const StudentHat = ({ hat }) => {
    let degree = hat.parameters.academicDegree == 1 ?
        "BSc" :
        hat.parameters.academicDegree == 2 ?
            "MSc" :
            "PHd";

    return (
        <div className='flex gap-x-2 gap-y-1 flex-wrap'>
            <HatParam>Student</HatParam>
            <HatParam>{degree}</HatParam>
            <HatParam>{hat.parameters.studyField}</HatParam>
        </div>
    )
}

export default StudentHat