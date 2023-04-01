import React from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import ProjectCard from '../comps/discover/ProjectCard';
import RefineButton from '../comps/discover/RefineButton';
import SearchFilter from '../comps/discover/SearchFilter';
import SearchFilters from '../comps/discover/SearchFilters';
import { useState } from 'react';
import RecommendedProjectCard from '../comps/discover/RecommendedProjectCard';
import SearchRefinements from '../comps/discover/SearchRefinements';

const Discover = () => {
    const projects = [
        {
            "id": "string",
            "datePosted": "01/02/03",
            "title": "platform development and maintenance",
            "description": "We are looking for new members to join the platform development team",
            "authorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "positions": [
                {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "datePosted": "2023-03-17T16:07:08.110Z",
                    "name": ".NET Backend Developer",
                    "description": "Develop and maintain platform backend",
                    "requirements": {
                        "type": "Student",
                        "parameters": {
                            "studyField": "Software Engineering",
                            "academicDegree": "Bachelor's"
                        }
                    }
                }
            ]
        },
        {
            "id": "string",
            "datePosted": "01/02/03",
            "title": "platform development and maintenance",
            "description": "We are looking for new members to join the platform development team",
            "authorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "positions": [
                {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "datePosted": "2023-03-17T16:07:08.110Z",
                    "name": ".NET Backend Developer",
                    "description": "Develop and maintain platform backend",
                    "requirements": {
                        "type": "Student",
                        "parameters": {
                            "studyField": "Software Engineering",
                            "academicDegree": "Bachelor's"
                        }
                    }
                },
                {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "datePosted": "2023-03-17T16:07:08.110Z",
                    "name": ".NET Backend Developer",
                    "description": "Develop and maintain platform backend",
                    "requirements": {
                        "type": "Student",
                        "parameters": {
                            "studyField": "Software Engineering",
                            "academicDegree": "Bachelor's"
                        }
                    }
                }
            ]
        }
    ];

    const [keyword, setKeyword] = useState(undefined);
    const [sort, setSort] = useState(undefined);
    const [hat, setHat] = useState(undefined);
    const [searchRefinementsVisibility, setSearchRefinementsVisibility] = useState(false);

    function updateProjectDiscoveryParameters(keyword, sort, hat) {
        setKeyword(keyword);
        setHat(hat);
        setSort(sort);
    }

    function showSearchRefinements() {
        setSearchRefinementsVisibility(true);
    }

    function hideSearchRefinements() {
        setSearchRefinementsVisibility(false);
    }

    return (
        <SingleColumnLayout
            title="Discover projects"
            description="Something encouraging here">

            <div className='mt-8 flex flex-col'>
                <RefineButton onClick={showSearchRefinements}></RefineButton>

                {/* filters */}
                <SearchFilters>
                    {
                        keyword != undefined &&
                        <SearchFilter
                            value={`keyword: ${keyword}`}
                            onRemoved={() => setKeyword(undefined)}>
                        </SearchFilter>
                    }
                    {
                        sort != undefined &&
                        <SearchFilter
                            value={`sort: ${sort}`}
                            onRemoved={() => setSort(undefined)}>
                        </SearchFilter>
                    }
                    {
                        hat != undefined &&
                        <SearchFilter
                            value={`fit for: my ${hat.type.toLowerCase()} hat`}
                            onRemoved={() => setHat(undefined)}>
                        </SearchFilter>
                    }
                </SearchFilters>
            </div>

            <div className='mt-16 flex flex-col space-y-8'>
                {
                    projects.map(p => <>
                        <div className=''>
                            <RecommendedProjectCard project={p}></RecommendedProjectCard>
                        </div>
                    </>)
                }
            </div>

            {
                searchRefinementsVisibility &&
                <div className='fixed left-0 top-0'>
                    <SearchRefinements
                        onModalClosed={hideSearchRefinements}
                        onSearchRefinementsChanged={updateProjectDiscoveryParameters}>
                    </SearchRefinements>
                </div>
            }
        </SingleColumnLayout>
    )
}

export default Discover