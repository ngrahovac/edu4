import React from 'react'
import HatParam from './HatParam'
import { BookOpenIcon } from '@heroicons/react/24/outline';
import { AcademicCapIcon } from '@heroicons/react/24/outline'


const StudentHat = (props) => {

    const { hat } = props;

    return (
        <div>
            <p
                className='text-slate-800 text-sm mb-2 font-semibold'>
                Student
            </p>

            <div
                className='flex flex-col'>

                <HatParam
                    text={
                        hat.parameters.academicDegree === 1 ? "BSc Studies" :
                            hat.parameters.academicDegree === 2 ? "MSc Studies" :
                                "PhD Studies"
                    }
                    icon={<AcademicCapIcon className="w-6 h-6 mr-2 fill-stone-100 stroke-slate-800 stroke-1"></AcademicCapIcon>}>
                </HatParam>

                <HatParam
                    text={hat.parameters.studyField}
                    icon={<BookOpenIcon className="w-6 h-6 mr-2 fill-stone-100 stroke-slate-800 stroke-1"></BookOpenIcon>}>
                </HatParam>
            </div>
        </div>
    )
}

export default StudentHat