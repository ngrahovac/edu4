import React from 'react'
import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import SpinnerLayout from '../layout/SpinnerLayout';
import { BeatLoader } from 'react-spinners';
import { useAuth0 } from '@auth0/auth0-react';
import ProjectCard from '../comps/discover/ProjectCard';
import SingleColumnLayout from '../layout/SingleColumnLayout';
import { me } from '../services/UsersService';
import { successResult, failureResult, errorResult } from '../services/RequestResult';
import { discover } from '../services/ProjectsService';
import ProjectSearchParam from '../comps/search/ProjectSearchParam'
import { HatSearchParam } from '../comps/search/HatSearchParam';

const Search = () => {

    const [search, setSearch] = useSearchParams();
    const [keyword, setKeyword] = useState(undefined);
    const [sort, setSort] = useState(undefined);
    const [hat, setHat] = useState(undefined);
    const [recommended, setRecommended] = useState(false);

    useEffect(() => {
        if (!recommended)
            setHat(undefined);
    }, [recommended])


    const [ownHats, setOwnHats] = useState(undefined);

    const [projects, setProjects] = useState(undefined);

    const [searchRefinementsFormVisible, setSearchRefinementsFormVisible] = useState(false);

    const [pageLoading, setPageLoading] = useState(true);
    const [projectsLoading, setProjectsLoading] = useState(true);

    const { getAccessTokenSilently } = useAuth0();

    useEffect(() => {
        console.log(search)
        console.log(search.get('hat'))
        setKeyword(search.get('keyword'));
        setSort(search.get('sort'));
        setHat(search.get('hat') ? ownHats.find(h => h.type == search.get('hat')) : undefined);
    }, [search])

    function setSearchParam(key, value) {
        const newParams = new URLSearchParams();

        search.forEach((value, k) => {
            if (k !== key) {
                newParams.append(k, value);
            }
        });

        if (value !== undefined)
            newParams.set(key, value);

        setSearch(newParams);
    }

    function toggleSearchRefinementsFormVisibility() {
        setSearchRefinementsFormVisible(!searchRefinementsFormVisible);
    }

    function handleDiscoveryRefinementsChange(keyword, sort, hat) {
        const newParams = new URLSearchParams();
        keyword && newParams.append('keyword', keyword);
        sort && newParams.append('sort', sort);
        hat && newParams.append('hat', hat.type);

        setSearch(newParams);
    }

    useEffect(() => {
        const fetchOwnHats = () => {
            (async () => {
                setPageLoading(true);

                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await me(token);
                    setPageLoading(false);

                    if (result.outcome === successResult) {
                        setOwnHats(result.payload.hats);
                    } else {
                        console.log("error fetching users hats");
                    }
                } catch (ex) {
                    console.log(ex);
                } finally {
                    setPageLoading(false);
                }
            })();
        }

        fetchOwnHats();
    }, [getAccessTokenSilently])

    useEffect(() => {
        const handleDiscoveryRefinementsChange = () => {
            (async () => {
                try {
                    setProjectsLoading(true);

                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await discover(keyword, sort, hat, token);
                    setProjectsLoading(false);

                    if (result.outcome === successResult) {
                        var projects = result.payload;
                        setProjects(projects);
                    } else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                } catch (ex) {
                    console.log("exception", ex);
                } finally {
                    setProjectsLoading(false);
                }
            })();
        }

        handleDiscoveryRefinementsChange();
    }, [keyword, sort, hat])

    if (pageLoading) {
        return (
            <SpinnerLayout>
                <BeatLoader
                    loading={pageLoading}
                    size={24}
                    color="blue">
                </BeatLoader>
            </SpinnerLayout>
        )
    }

    const searchResults = projectsLoading ?
        <BeatLoader></BeatLoader> :
        projects && projects.length ?
            <div className='flex flex-col space-y-8'>
                {
                    projects.map((p, index) => <div key={index}>
                        <ProjectCard project={p} ownHats={ownHats}></ProjectCard>
                    </div>)
                }
            </div> :
            <p>There are currently no projects satisfying the criteria.</p>;

    return (
        ownHats &&

        <SingleColumnLayout
            title="Discover projects"
            description="Search projects and find opportunities to contribute to bla bla">
            <div className='flex flex-col gap-y-16'>
                <div className='relative flex drop-shadow-md rounded-full bg-white px-6 py-4 gap-x-4 items-center mx-auto w-1/2'>
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="lightgray" className="w-6 h-6 absolute left-6">
                        <path strokeLinecap="round" strokeLinejoin="round" d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z" />
                    </svg>
                    <input
                        className='text-center w-full'
                        value={keyword}>
                    </input>
                </div>

                <div className='flex flex-col gap-y-4'>
                    <div className='flex justify-between items-center'>
                        {/*
                        <label className="inline-flex items-center cursor-pointer gap-x-2">
                            <input type="checkbox" value="" className="sr-only peer" />
                            <div className="relative w-11 h-6 bg-gray-200 rounded-full peer peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600 peer-checked:bg-indigo-500"></div>
                            <span className="text-gray-700">Recommended only</span>
                        </label>

                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="gray" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M3 7.5 7.5 3m0 0L12 7.5M7.5 3v13.5m13.5 0L16.5 21m0 0L12 16.5m4.5 4.5V7.5" />
                        </svg>
    */}
                        <label className="inline-flex items-center cursor-pointer gap-x-2">
                            <input type="checkbox" value={recommended} className="sr-only peer" checked={recommended} onChange={() => setRecommended(!recommended)} />
                            <div className="relative w-11 h-6 bg-gray-200 rounded-full peer peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600 peer-checked:bg-indigo-500"></div>
                            <span className="text-gray-700">Recommended only</span>
                        </label>

                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="gray" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M3 7.5 7.5 3m0 0L12 7.5M7.5 3v13.5m13.5 0L16.5 21m0 0L12 16.5m4.5 4.5V7.5" />
                        </svg>
                    </div>

                    <div className={`flex gap-x-4 items-center text-gray-600 ${recommended ? "visible" : "hidden"}`}>
                        Looking for a
                        {
                            ownHats.map(h => (
                                <HatSearchParam
                                    selected={hat != undefined && hat.type == h.type}
                                    onSelected={() => { setHat(h) }}
                                >
                                    {h.type}
                                </HatSearchParam>
                            ))
                        }
                        like me
                    </div>

                    {searchResults}
                </div>
            </div>
        </SingleColumnLayout >
    )
}

export default Search