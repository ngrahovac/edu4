import React from 'react'
import HatParam from './HatParam';

const StudentHat = (props) => {
    const {
        hat,
        ownHats
    } = props;

    let degree = hat.parameters.academicDegree == 1 ?
        "BSc" :
        hat.parameters.academicDegree == 2 ?
            "MSc" :
            "PHd";

    let typeMatch = ownHats.find(h => h.type == "Student") != undefined;
    let degreeMatch = typeMatch && ownHats.find(h => h.type == "Student" && h.parameters.academicDegree <= 3) != undefined; 
    let fieldMatch = ownHats.find(h => h.parameters.researchField == hat.parameters.studyField || h.parameters.studyField == hat.parameters.studyField);
    
    return (
        <div className='flex gap-x-2 gap-y-1 flex-wrap'>
            <HatParam match={typeMatch}>Student</HatParam>
            <HatParam match={degreeMatch}>{degree}</HatParam>
            <HatParam match={fieldMatch}>{hat.parameters.studyField}</HatParam>
        </div>
    )
}

export default StudentHat